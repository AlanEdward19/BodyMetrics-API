using MongoDB.Driver;

namespace BodyMetricsApi.Infrastructure.Persistence;

public sealed class MongoDbIndexesHostedService(MongoDbContext mongoDbContext) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var athleteOwnerIndex = new CreateIndexModel<Features.Athletes.Athlete>(
            Builders<Features.Athletes.Athlete>.IndexKeys.Ascending(x => x.OwnerUserId));

        var athleteSportIndex = new CreateIndexModel<Features.Athletes.Athlete>(
            Builders<Features.Athletes.Athlete>.IndexKeys.Ascending(x => x.OwnerUserId).Ascending(x => x.SportId));

        var athleteFullNameIndex = new CreateIndexModel<Features.Athletes.Athlete>(
            Builders<Features.Athletes.Athlete>.IndexKeys.Ascending(x => x.OwnerUserId).Ascending(x => x.FullName));

        await mongoDbContext.Athletes.Indexes.CreateManyAsync(
            [athleteOwnerIndex, athleteSportIndex, athleteFullNameIndex],
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

