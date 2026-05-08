using BodyMetricsApi.Shared.Results;

namespace BodyMetricsApi.Features.Sports.Delete;

public sealed class DeleteSportCommandHandler(ISportRepository repository)
{
    public async Task<OperationResult> HandleAsync(DeleteSportCommand command, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(command.Id, cancellationToken);
        return deleted
            ? OperationResult.Success()
            : OperationResult.NotFound($"Sport '{command.Id}' was not found.");
    }
}


