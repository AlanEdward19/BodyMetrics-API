using BodyMetricsApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BodyMetricsApi.Features.Sports;

public sealed class EfSportRepository(BodyMetricsDbContext dbContext) : ISportRepository
{
    public async Task<IReadOnlyList<Sport>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Sports
            .AsNoTracking()
            .OrderBy(sport => sport.Name)
            .ToListAsync(cancellationToken);
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



