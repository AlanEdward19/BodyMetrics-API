using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BodyMetricsApi.Features.AthleteGroups.Create;

public sealed class CreateAthleteGroupCommandHandler(
    IAthleteGroupRepository groupRepository,
    ICurrentUserService currentUserService,
    IValidator<CreateAthleteGroupCommand> validator)
{
    public async Task<OperationResult<AthleteGroupViewModel>> HandleAsync(CreateAthleteGroupCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationResult<AthleteGroupViewModel>.Validation(validationResult.ToErrorDictionary());
        }

        var nameExists = await groupRepository.ExistsByNameAsync(currentUserService.UserId, command.Name, null, cancellationToken);
        if (nameExists)
        {
            return OperationResult<AthleteGroupViewModel>.Validation(new Dictionary<string, string[]>
            {
                [nameof(command.Name)] = ["A group with this name already exists."]
            });
        }

        var group = AthleteGroup.Create(currentUserService.UserId, command.Name);
        await groupRepository.AddAsync(group, cancellationToken);

        return OperationResult<AthleteGroupViewModel>.Success(group.ToViewModel(), StatusCodes.Status201Created);
    }
}
