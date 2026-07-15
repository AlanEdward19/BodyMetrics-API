using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace BodyMetricsApi.Features.AthleteGroups.RemoveMember;

public sealed class RemoveAthleteFromGroupCommandHandler(
    IAthleteGroupRepository groupRepository,
    IAthleteRepository athleteRepository,
    ICurrentUserService currentUserService)
{
    public async Task<OperationResult> HandleAsync(RemoveAthleteFromGroupCommand command, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(command.GroupId, currentUserService.UserId, cancellationToken);
        if (group is null)
        {
            return OperationResult.NotFound("Athlete group not found.");
        }

        var removedAthlete = group.RemoveMember(command.AthleteId);
        if (removedAthlete is null)
        {
            return OperationResult.Success(StatusCodes.Status204NoContent);
        }

        await groupRepository.UpdateAsync(group, cancellationToken);
        await athleteRepository.AddAsync(removedAthlete, cancellationToken);

        return OperationResult.Success(StatusCodes.Status204NoContent);
    }
}
