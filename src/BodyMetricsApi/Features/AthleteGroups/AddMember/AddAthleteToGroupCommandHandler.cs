using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BodyMetricsApi.Features.AthleteGroups.AddMember;

public sealed class AddAthleteToGroupCommandHandler(
    IAthleteGroupRepository groupRepository,
    IAthleteRepository athleteRepository,
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

        var athlete = await athleteRepository.GetByIdAsync(command.AthleteId, currentUserService.UserId, cancellationToken);
        if (athlete is null)
        {
            return OperationResult.NotFound("Athlete not found.");
        }

        group.AddMember(command.AthleteId);
        await groupRepository.UpdateAsync(group, cancellationToken);

        return OperationResult.Success(StatusCodes.Status204NoContent);
    }
}
