using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BodyMetricsApi.Features.AthleteGroups.Shared.Persistence;

public sealed class EfAthleteGroupRepository(BodyMetricsDbContext dbContext) : IAthleteGroupRepository
{
    public async Task AddAsync(AthleteGroup group, CancellationToken cancellationToken)
    {
        await dbContext.AthleteGroups.AddAsync(group, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AthleteGroup?> GetByIdAsync(string id, string ownerUserId, CancellationToken cancellationToken)
    {
        return await dbContext.AthleteGroups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id && g.OwnerUserId == ownerUserId, cancellationToken);
    }

    public async Task<List<AthleteGroup>> GetAllByOwnerAsync(string ownerUserId, CancellationToken cancellationToken)
    {
        return await dbContext.AthleteGroups
            .AsNoTracking()
            .Where(g => g.OwnerUserId == ownerUserId)
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(AthleteGroup group, CancellationToken cancellationToken)
    {
        var existing = await dbContext.AthleteGroups
            .FirstOrDefaultAsync(g => g.Id == group.Id && g.OwnerUserId == group.OwnerUserId, cancellationToken);

        if (existing is null)
        {
            throw new InvalidOperationException($"AthleteGroup '{group.Id}' could not be updated.");
        }

        dbContext.AthleteGroups.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.AthleteGroups.AddAsync(group, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, string ownerUserId, CancellationToken cancellationToken)
    {
        var group = await dbContext.AthleteGroups
            .FirstOrDefaultAsync(g => g.Id == id && g.OwnerUserId == ownerUserId, cancellationToken);

        if (group is null)
        {
            return false;
        }

        dbContext.AthleteGroups.Remove(group);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string ownerUserId, string name, string? excludeId, CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim().ToUpperInvariant();
        return await dbContext.AthleteGroups
            .AsNoTracking()
            .AnyAsync(g =>
                g.OwnerUserId == ownerUserId &&
                g.Name.ToUpper() == normalizedName &&
                (excludeId == null || g.Id != excludeId),
                cancellationToken);
    }
}
