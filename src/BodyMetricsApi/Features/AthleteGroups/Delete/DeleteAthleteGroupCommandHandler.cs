using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace BodyMetricsApi.Features.AthleteGroups.Delete;

public sealed class DeleteAthleteGroupCommandHandler(
    IAthleteGroupRepository groupRepository,
    ICurrentUserService currentUserService)
{
    public async Task<OperationResult> HandleAsync(DeleteAthleteGroupCommand command, CancellationToken cancellationToken)
    {
        var deleted = await groupRepository.DeleteAsync(command.Id, currentUserService.UserId, cancellationToken);
        if (!deleted)
        {
            return OperationResult.NotFound("Athlete group not found.");
        }

        return OperationResult.Success(StatusCodes.Status204NoContent);
    }
}
