using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.Validation;
using FluentValidation;

namespace BodyMetricsApi.Features.AthleteGroups.Update;

public sealed class UpdateAthleteGroupCommandHandler(
    IAthleteGroupRepository groupRepository,
    ICurrentUserService currentUserService,
    IValidator<UpdateAthleteGroupCommand> validator)
{
    public async Task<OperationResult<AthleteGroupViewModel>> HandleAsync(UpdateAthleteGroupCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationResult<AthleteGroupViewModel>.Validation(validationResult.ToErrorDictionary());
        }

        var group = await groupRepository.GetByIdAsync(command.Id, currentUserService.UserId, cancellationToken);
        if (group is null)
        {
            return OperationResult<AthleteGroupViewModel>.NotFound("Athlete group not found.");
        }

        var nameExists = await groupRepository.ExistsByNameAsync(currentUserService.UserId, command.Name, command.Id, cancellationToken);
        if (nameExists)
        {
            return OperationResult<AthleteGroupViewModel>.Validation(new Dictionary<string, string[]>
            {
                [nameof(command.Name)] = ["A group with this name already exists."]
            });
        }

        group.Rename(command.Name);
        await groupRepository.UpdateAsync(group, cancellationToken);

        return OperationResult<AthleteGroupViewModel>.Success(group.ToViewModel());
    }
}
