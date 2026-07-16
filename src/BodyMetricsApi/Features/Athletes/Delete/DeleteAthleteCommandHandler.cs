using BodyMetricsApi.Features.Athletes.Shared;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;

namespace BodyMetricsApi.Features.Athletes.Delete;

public sealed class DeleteAthleteCommandHandler(AthleteLocator athleteLocator, ICurrentUserService currentUserService)
{
    public async Task<OperationResult> HandleAsync(DeleteAthleteCommand command, CancellationToken cancellationToken)
    {
        var location = await athleteLocator.FindAsync(command.Id, currentUserService.UserId, cancellationToken);
        if (location is null)
        {
            return OperationResult.NotFound($"Athlete '{command.Id}' was not found.");
        }

        await athleteLocator.DetachAsync(location, cancellationToken);
        return OperationResult.Success();
    }
}
