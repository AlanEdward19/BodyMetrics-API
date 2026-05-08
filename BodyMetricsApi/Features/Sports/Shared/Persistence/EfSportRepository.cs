using BodyMetricsApi.Features.Sports.Shared.Interfaces;
using BodyMetricsApi.Infrastructure.Persistence;
using BodyMetricsApi.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BodyMetricsApi.Features.Sports.Shared.Persistence;

public sealed class EfSportRepository(BodyMetricsDbContext dbContext) : ISportRepository
{
    public async Task<PagedResultDto<Sport>> GetAllAsync(
        int page,
        int pageSize,
        string? name,
        string? sector,
        string? category,
        CancellationToken cancellationToken)
    {
        var sports = await dbContext.Sports
            .AsNoTracking()
            .OrderBy(sport => sport.Name)
            .ToListAsync(cancellationToken);

        IEnumerable<Sport> filtered = sports;

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalizedName = name.Trim();
            filtered = filtered.Where(sport => sport.Name.Contains(normalizedName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(sector))
        {
            var normalizedSector = sector.Trim();
            filtered = filtered.Where(sport => sport.SupportsSector(normalizedSector));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim();
            filtered = filtered.Where(sport => sport.SupportsCategory(normalizedCategory));
        }

        var filteredList = filtered.ToList();
        var pagedItems = filteredList
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResultDto<Sport>(pagedItems, filteredList.Count);
    }

    public async Task<Sport?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await dbContext.Sports
            .AsNoTracking()
            .FirstOrDefaultAsync(sport => sport.Id == id, cancellationToken);
    }

    public async Task AddAsync(Sport sport, CancellationToken cancellationToken)
    {
        await dbContext.Sports.AddAsync(sport, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceAsync(Sport sport, CancellationToken cancellationToken)
    {
        dbContext.Sports.Update(sport);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var sport = await dbContext.Sports.FirstOrDefaultAsync(current => current.Id == id, cancellationToken);
        if (sport is null)
        {
            return false;
        }

        dbContext.Sports.Remove(sport);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

