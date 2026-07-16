using BodyMetricsApi.Features.Athletes;
using BodyMetricsApi.Features.AthleteGroups;
using BodyMetricsApi.Features.Sports;
using BodyMetricsApi.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BodyMetricsApi.Infrastructure.Persistence;

public sealed class MongoDbContext
{
    public MongoDbContext(IOptions<MongoDbOptions> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        Database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoDatabase Database { get; }

    public IMongoCollection<Sport> Sports => Database.GetCollection<Sport>("sports");

    public IMongoCollection<Athlete> Athletes => Database.GetCollection<Athlete>("athletes");

    public IMongoCollection<AthleteGroup> AthleteGroups => Database.GetCollection<AthleteGroup>("athleteGroups");
}

