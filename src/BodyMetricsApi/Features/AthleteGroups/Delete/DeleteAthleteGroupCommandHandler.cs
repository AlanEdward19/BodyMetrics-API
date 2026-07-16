using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace BodyMetricsApi.Features.AthleteGroups.Delete;

public sealed class DeleteAthleteGroupCommandHandler(
    IAthleteGroupRepository groupRepository,
    IAthleteRepository athleteRepository,
    ICurrentUserService currentUserService)
{
    public async Task<OperationResult> HandleAsync(DeleteAthleteGroupCommand command, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(command.Id, currentUserService.UserId, cancellationToken);
        if (group is null)
        {
            return OperationResult.NotFound("Athlete group not found.");
        }

        // Deleting a group must not delete its athletes - they return to being standalone.
        if (group.Members.Count > 0)
        {
            await athleteRepository.AddRangeAsync(group.Members, cancellationToken);
        }

        await groupRepository.DeleteAsync(command.Id, currentUserService.UserId, cancellationToken);

        return OperationResult.Success(StatusCodes.Status204NoContent);
    }
}
