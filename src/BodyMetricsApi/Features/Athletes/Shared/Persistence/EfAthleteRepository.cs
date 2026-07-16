using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Infrastructure.Persistence;
using BodyMetricsApi.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BodyMetricsApi.Features.Athletes.Shared.Persistence;

public sealed class EfAthleteRepository(BodyMetricsDbContext dbContext) : IAthleteRepository
{
    public async Task<PagedResultDto<Athlete>> GetAllAsync(
        string ownerUserId,
        int page,
        int pageSize,
        string? fullName,
        string? sportId,
        string? sector,
        string? category,
        Features.Athletes.Shared.Enums.Phase? phase,
        CancellationToken cancellationToken)
    {
        var athletes = await GetAllRawAsync(ownerUserId, cancellationToken);
        var filtered = AthleteFilter.Apply(athletes, fullName, sportId, sector, category, phase);
        var pagedItems = AthleteFilter.Paginate(filtered, page, pageSize, out var totalCount);

        return new PagedResultDto<Athlete>(pagedItems, totalCount);
    }

    public async Task<List<Athlete>> GetAllRawAsync(string ownerUserId, CancellationToken cancellationToken)
    {
        return await dbContext.Athletes
            .AsNoTracking()
            .Where(athlete => athlete.OwnerUserId == ownerUserId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Athlete?> GetByIdAsync(string id, string ownerUserId, CancellationToken cancellationToken)
    {
        return await dbContext.Athletes
            .AsNoTracking()
            .FirstOrDefaultAsync(athlete => athlete.Id == id && athlete.OwnerUserId == ownerUserId, cancellationToken);
    }

    public async Task<Athlete?> GetByFullNameAsync(string ownerUserId, string fullName, CancellationToken cancellationToken)
    {
        var normalizedFullName = fullName.Trim().ToUpperInvariant();

        return await dbContext.Athletes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                athlete => athlete.OwnerUserId == ownerUserId && athlete.FullName.ToUpper() == normalizedFullName,
                cancellationToken);
    }

    public async Task<IReadOnlyDictionary<string, Athlete>> GetByFullNamesAsync(
        string ownerUserId, IReadOnlyCollection<string> fullNames, CancellationToken cancellationToken)
    {
        var normalizedNames = fullNames
            .Select(name => name.Trim().ToUpperInvariant())
            .Distinct()
            .ToList();

        var athletes = await dbContext.Athletes
            .AsNoTracking()
            .Where(athlete => athlete.OwnerUserId == ownerUserId && normalizedNames.Contains(athlete.FullName.ToUpper()))
            .ToListAsync(cancellationToken);

        return athletes
            .GroupBy(athlete => athlete.FullName.Trim().ToUpperInvariant())
            .ToDictionary(group => group.Key, group => group.First());
    }

    public async Task AddAsync(Athlete athlete, CancellationToken cancellationToken)
    {
        await dbContext.Athletes.AddAsync(athlete, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceAsync(Athlete athlete, CancellationToken cancellationToken)
    {
        var existingAthlete = await dbContext.Athletes
            .FirstOrDefaultAsync(
                current => current.Id == athlete.Id && current.OwnerUserId == athlete.OwnerUserId,
                cancellationToken);

        if (existingAthlete is null)
        {
            throw new InvalidOperationException($"Athlete '{athlete.Id}' could not be replaced.");
        }

        dbContext.Athletes.Remove(existingAthlete);
        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Athletes.AddAsync(athlete, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IReadOnlyCollection<Athlete> athletes, CancellationToken cancellationToken)
    {
        // MongoDB multi-document transactions require a replica set; EF Core's default of
        // wrapping multi-entity SaveChanges in an ambient transaction breaks on a standalone
        // instance, so it's disabled for this bulk path (each insert is already atomic on its own).
        dbContext.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        await dbContext.Athletes.AddRangeAsync(athletes, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceRangeAsync(IReadOnlyCollection<Athlete> athletes, CancellationToken cancellationToken)
    {
        dbContext.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;

        var ids = athletes.Select(athlete => athlete.Id).ToList();
        var existingAthletes = await dbContext.Athletes
            .Where(athlete => ids.Contains(athlete.Id))
            .ToListAsync(cancellationToken);

        dbContext.Athletes.RemoveRange(existingAthletes);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Athletes.AddRangeAsync(athletes, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, string ownerUserId, CancellationToken cancellationToken)
    {
        var athlete = await dbContext.Athletes.FirstOrDefaultAsync(current => current.Id == id && current.OwnerUserId == ownerUserId, cancellationToken);
        if (athlete is null)
        {
            return false;
        }

        dbContext.Athletes.Remove(athlete);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

