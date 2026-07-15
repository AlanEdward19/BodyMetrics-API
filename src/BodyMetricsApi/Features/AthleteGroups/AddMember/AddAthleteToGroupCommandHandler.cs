using BodyMetricsApi.Features.Athletes.Shared;
using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BodyMetricsApi.Features.AthleteGroups.AddMember;

public sealed class AddAthleteToGroupCommandHandler(
    IAthleteGroupRepository groupRepository,
    AthleteLocator athleteLocator,
    ICurrentUserService currentUserService,
    IValidator<AddAthleteToGroupCommand> validator)
{
    public async Task<OperationResult> HandleAsync(AddAthleteToGroupCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationResult.Validation(validationResult.ToErrorDictionary());
        }

        var group = await groupRepository.GetByIdAsync(command.GroupId, currentUserService.UserId, cancellationToken);
        if (group is null)
        {
            return OperationResult.NotFound("Athlete group not found.");
        }

        if (group.Members.Any(m => m.Id == command.AthleteId))
        {
            return OperationResult.Success(StatusCodes.Status204NoContent);
        }

        // Locates the athlete wherever it currently lives - standalone or embedded in
        // another group - so adding to this group also acts as "move between groups".
        var location = await athleteLocator.FindAsync(command.AthleteId, currentUserService.UserId, cancellationToken);
        if (location is null)
        {
            return OperationResult.NotFound("Athlete not found.");
        }

        await athleteLocator.DetachAsync(location, cancellationToken);
        group.AddMember(location.Athlete);
        await groupRepository.UpdateAsync(group, cancellationToken);

        return OperationResult.Success(StatusCodes.Status204NoContent);
    }
}
