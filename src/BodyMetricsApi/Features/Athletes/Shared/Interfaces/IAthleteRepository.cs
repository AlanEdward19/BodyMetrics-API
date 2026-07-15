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

    Task<List<Athlete>> GetAllRawAsync(string ownerUserId, CancellationToken cancellationToken);

    Task<Athlete?> GetByIdAsync(string id, string ownerUserId, CancellationToken cancellationToken);

    Task<Athlete?> GetByFullNameAsync(string ownerUserId, string fullName, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, Athlete>> GetByFullNamesAsync(
        string ownerUserId, IReadOnlyCollection<string> fullNames, CancellationToken cancellationToken);

    Task AddAsync(Athlete athlete, CancellationToken cancellationToken);

    Task ReplaceAsync(Athlete athlete, CancellationToken cancellationToken);

    Task AddRangeAsync(IReadOnlyCollection<Athlete> athletes, CancellationToken cancellationToken);

    Task ReplaceRangeAsync(IReadOnlyCollection<Athlete> athletes, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string id, string ownerUserId, CancellationToken cancellationToken);
}

