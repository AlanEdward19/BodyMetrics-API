using System.Text.RegularExpressions;
using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Infrastructure.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BodyMetricsApi.Features.AthleteGroups.Shared.Persistence;

// Goes through the raw MongoDB driver rather than the EF context: AthleteGroup embeds
// Athlete (an EF entity in its own right), and EF's relational change tracking can't
// cleanly reconcile that dual role - plain BSON document reads/replaces avoid it entirely.
public sealed class MongoAthleteGroupRepository(MongoDbContext mongoDbContext) : IAthleteGroupRepository
{
    public async Task AddAsync(AthleteGroup group, CancellationToken cancellationToken)
    {
        await mongoDbContext.AthleteGroups.InsertOneAsync(group, cancellationToken: cancellationToken);
    }

    public async Task<AthleteGroup?> GetByIdAsync(string id, string ownerUserId, CancellationToken cancellationToken)
    {
        return await mongoDbContext.AthleteGroups
            .Find(g => g.Id == id && g.OwnerUserId == ownerUserId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<AthleteGroup>> GetAllByOwnerAsync(string ownerUserId, CancellationToken cancellationToken)
    {
        return await mongoDbContext.AthleteGroups
            .Find(g => g.OwnerUserId == ownerUserId)
            .SortBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<AthleteGroup?> FindByMemberIdAsync(string ownerUserId, string athleteId, CancellationToken cancellationToken)
    {
        var filter = Builders<AthleteGroup>.Filter.And(
            Builders<AthleteGroup>.Filter.Eq(g => g.OwnerUserId, ownerUserId),
            Builders<AthleteGroup>.Filter.ElemMatch(g => g.Members, m => m.Id == athleteId));

        return await mongoDbContext.AthleteGroups.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(AthleteGroup group, CancellationToken cancellationToken)
    {
        await mongoDbContext.AthleteGroups.ReplaceOneAsync(
            g => g.Id == group.Id && g.OwnerUserId == group.OwnerUserId,
            group,
            cancellationToken: cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, string ownerUserId, CancellationToken cancellationToken)
    {
        var result = await mongoDbContext.AthleteGroups.DeleteOneAsync(
            g => g.Id == id && g.OwnerUserId == ownerUserId, cancellationToken);

        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsByNameAsync(string ownerUserId, string name, string? excludeId, CancellationToken cancellationToken)
    {
        var exactNameFilter = new BsonRegularExpression($"^{Regex.Escape(name.Trim())}$", "i");
        var filter = Builders<AthleteGroup>.Filter.And(
            Builders<AthleteGroup>.Filter.Eq(g => g.OwnerUserId, ownerUserId),
            Builders<AthleteGroup>.Filter.Regex(g => g.Name, exactNameFilter));

        if (excludeId is not null)
        {
            filter &= Builders<AthleteGroup>.Filter.Ne(g => g.Id, excludeId);
        }

        return await mongoDbContext.AthleteGroups.Find(filter).AnyAsync(cancellationToken);
    }
}
