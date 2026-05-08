using BodyMetricsApi.Features.Sports.Shared.Interfaces;
using BodyMetricsApi.Features.Sports.Shared.ViewModels;

namespace BodyMetricsApi.Features.Sports.GetAll;

public sealed class GetAllSportsQueryHandler(ISportRepository repository)
{
    public async Task<IReadOnlyList<SportResponse>> HandleAsync(GetAllSportsQuery query, CancellationToken cancellationToken)
    {
        var sports = await repository.GetAllAsync(cancellationToken);
        return sports.Select(sport => sport.ToResponse()).ToList();
    }
}

