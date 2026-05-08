using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;

namespace BodyMetricsApi.Features.Athletes.Delete;

public sealed class DeleteAthleteCommandHandler(IAthleteRepository athleteRepository, ICurrentUserService currentUserService)
{
    public async Task<OperationResult> HandleAsync(DeleteAthleteCommand command, CancellationToken cancellationToken)
    {
        var deleted = await athleteRepository.DeleteAsync(command.Id, currentUserService.UserId, cancellationToken);
        return deleted
            ? OperationResult.Success()
            : OperationResult.NotFound($"Athlete '{command.Id}' was not found.");
    }
}



