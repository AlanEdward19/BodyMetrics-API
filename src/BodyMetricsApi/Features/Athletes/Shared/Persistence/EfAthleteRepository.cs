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
        var athletes = await dbContext.Athletes
            .AsNoTracking()
            .Where(athlete => athlete.OwnerUserId == ownerUserId)
            .OrderBy(athlete => athlete.FullName)
            .ToListAsync(cancellationToken);

        IEnumerable<Athlete> filtered = athletes;

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            var normalizedFullName = NormalizeSearchTerm(fullName);
            filtered = filtered.Where(athlete => MatchesFullNameSearch(athlete.FullName, normalizedFullName));
        }

        if (!string.IsNullOrWhiteSpace(sportId))
        {
            var normalizedSportId = sportId.Trim();
            filtered = filtered.Where(athlete => string.Equals(athlete.SportId, normalizedSportId, StringComparison.Ordinal));
        }

        if (!string.IsNullOrWhiteSpace(sector))
        {
            var normalizedSector = sector.Trim();
            filtered = filtered.Where(athlete => string.Equals(athlete.Sector, normalizedSector, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim();
            filtered = filtered.Where(athlete => string.Equals(athlete.Category, normalizedCategory, StringComparison.OrdinalIgnoreCase));
        }

        if (phase.HasValue)
        {
            filtered = filtered.Where(athlete => athlete.Phase == phase.Value);
        }

        var filteredList = filtered.ToList();
        var pagedItems = filteredList
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResultDto<Athlete>(pagedItems, filteredList.Count);
    }

    public async Task<Athlete?> GetByIdAsync(string id, string ownerUserId, CancellationToken cancellationToken)
    {
        return await dbContext.Athletes
            .AsNoTracking()
            .FirstOrDefaultAsync(athlete => athlete.Id == id && athlete.OwnerUserId == ownerUserId, cancellationToken);
    }

    public async Task<Athlete?> GetByFullNameAsync(string ownerUserId, string fullName, CancellationToken cancellationToken)
    {
        var normalizedFullName = fullName.Trim();

        var athletes = await dbContext.Athletes
            .AsNoTracking()
            .Where(athlete => athlete.OwnerUserId == ownerUserId)
            .OrderBy(athlete => athlete.FullName)
            .ToListAsync(cancellationToken);

        return athletes.FirstOrDefault(athlete => string.Equals(athlete.FullName, normalizedFullName, StringComparison.OrdinalIgnoreCase));
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

    private static bool MatchesFullNameSearch(string fullName, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return true;
        }

        var normalizedFullName = NormalizeSearchTerm(fullName);
        if (normalizedFullName.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return normalizedFullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Any(token => token.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeSearchTerm(string value)
    {
        return string.Join(' ', value
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}

