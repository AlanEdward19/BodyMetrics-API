using BodyMetricsApi.Features.Athletes.Shared.Enums;
using BodyMetricsApi.Shared.Dtos;

namespace BodyMetricsApi.Features.Athletes.Shared.Interfaces;

public interface IAthleteRepository
{
    Task<PagedResultDto<Athlete>> GetAllAsync(
        string ownerUserId,
        int page,
        int pageSize,
        string? fullName,
        string? sportId,
        string? sector,
        string? category,
        Phase? phase,
        CancellationToken cancellationToken);

    Task<Athlete?> GetByIdAsync(string id, string ownerUserId, CancellationToken cancellationToken);

    Task AddAsync(Athlete athlete, CancellationToken cancellationToken);

    Task ReplaceAsync(Athlete athlete, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string id, string ownerUserId, CancellationToken cancellationToken);
}

