using MongoDB.Driver;

namespace BodyMetricsApi.Infrastructure.Persistence;

public sealed class MongoDbIndexesHostedService(MongoDbContext mongoDbContext) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var athleteSportIndex = new CreateIndexModel<Features.Athletes.Athlete>(
            Builders<Features.Athletes.Athlete>.IndexKeys.Ascending(x => x.SportId));

        await mongoDbContext.Athletes.Indexes.CreateOneAsync(athleteSportIndex, cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

