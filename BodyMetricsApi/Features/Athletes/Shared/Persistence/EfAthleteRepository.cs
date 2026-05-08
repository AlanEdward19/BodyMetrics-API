using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BodyMetricsApi.Features.Athletes.Shared.Persistence;

public sealed class EfAthleteRepository(BodyMetricsDbContext dbContext) : IAthleteRepository
{
    public async Task<IReadOnlyList<Athlete>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Athletes
            .AsNoTracking()
            .OrderBy(athlete => athlete.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Athlete?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await dbContext.Athletes
            .AsNoTracking()
            .FirstOrDefaultAsync(athlete => athlete.Id == id, cancellationToken);
    }

    public async Task AddAsync(Athlete athlete, CancellationToken cancellationToken)
    {
        await dbContext.Athletes.AddAsync(athlete, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceAsync(Athlete athlete, CancellationToken cancellationToken)
    {
        dbContext.Athletes.Update(athlete);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var athlete = await dbContext.Athletes.FirstOrDefaultAsync(current => current.Id == id, cancellationToken);
        if (athlete is null)
        {
            return false;
        }

        dbContext.Athletes.Remove(athlete);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

