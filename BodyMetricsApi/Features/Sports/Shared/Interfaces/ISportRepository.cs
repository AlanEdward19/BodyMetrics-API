using BodyMetricsApi.Shared.Dtos;

namespace BodyMetricsApi.Features.Sports.Shared.Interfaces;

public interface ISportRepository
{
    Task<PagedResultDto<Sport>> GetAllAsync(
        int page,
        int pageSize,
        string? name,
        string? sector,
        string? category,
        CancellationToken cancellationToken);

    Task<Sport?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task AddAsync(Sport sport, CancellationToken cancellationToken);

    Task ReplaceAsync(Sport sport, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}

