namespace BodyMetricsApi.Features.Athletes.Shared.Interfaces;

public interface IAthleteRepository
{
    Task<IReadOnlyList<Athlete>> GetAllAsync(CancellationToken cancellationToken);

    Task<Athlete?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task AddAsync(Athlete athlete, CancellationToken cancellationToken);

    Task ReplaceAsync(Athlete athlete, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}

