using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace BodyMetricsApi.Features.AthleteGroups.RemoveMember;

public sealed class RemoveAthleteFromGroupCommandHandler(
    IAthleteGroupRepository groupRepository,
    ICurrentUserService currentUserService)
{
    public async Task<OperationResult> HandleAsync(RemoveAthleteFromGroupCommand command, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(command.GroupId, currentUserService.UserId, cancellationToken);
        if (group is null)
        {
            return OperationResult.NotFound("Athlete group not found.");
        }

        group.RemoveMember(command.AthleteId);
        await groupRepository.UpdateAsync(group, cancellationToken);

        return OperationResult.Success(StatusCodes.Status204NoContent);
    }
}
