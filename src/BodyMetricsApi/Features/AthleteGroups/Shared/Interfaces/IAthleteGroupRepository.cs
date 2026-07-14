namespace BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;

public interface IAthleteGroupRepository
{
    Task AddAsync(AthleteGroup group, CancellationToken cancellationToken);

    Task<AthleteGroup?> GetByIdAsync(string id, string ownerUserId, CancellationToken cancellationToken);

    Task<List<AthleteGroup>> GetAllByOwnerAsync(string ownerUserId, CancellationToken cancellationToken);

    Task UpdateAsync(AthleteGroup group, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string id, string ownerUserId, CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(string ownerUserId, string name, string? excludeId, CancellationToken cancellationToken);
}
