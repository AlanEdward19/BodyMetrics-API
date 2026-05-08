namespace BodyMetricsApi.Features.Sports.Shared.Interfaces;

public interface ISportRepository
{
    Task<IReadOnlyList<Sport>> GetAllAsync(CancellationToken cancellationToken);

    Task<Sport?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task AddAsync(Sport sport, CancellationToken cancellationToken);

    Task ReplaceAsync(Sport sport, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}

