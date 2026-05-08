using BodyMetricsApi.Shared.Results;

namespace BodyMetricsApi.Features.Sports.GetById;

public sealed class GetSportByIdQueryHandler(ISportRepository repository)
{
    public async Task<OperationResult<SportResponse>> HandleAsync(GetSportByIdQuery query, CancellationToken cancellationToken)
    {
        var sport = await repository.GetByIdAsync(query.Id, cancellationToken);
        return sport is null
            ? OperationResult<SportResponse>.NotFound($"Sport '{query.Id}' was not found.")
            : OperationResult<SportResponse>.Success(sport.ToResponse());
    }
}


