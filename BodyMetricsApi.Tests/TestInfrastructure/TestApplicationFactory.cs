using System.Text.Json;
using System.Text.Json.Serialization;
using BodyMetricsApi.Infrastructure.Configuration;
using BodyMetricsApi.Infrastructure.Serialization;
using BodyMetricsApi.Infrastructure.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BodyMetricsApi.Tests.TestInfrastructure;

public sealed class TestApplicationFactory(MongoContainerFixture mongoFixture) : WebApplicationFactory<Program>
{
    public JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(), new DateOnlyJsonConverter() }
    };

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        var databaseName = $"bodymetrics-tests-{Guid.NewGuid():N}";

        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{MongoDbOptions.SectionName}:ConnectionString"] = mongoFixture.ConnectionString,
                [$"{MongoDbOptions.SectionName}:DatabaseName"] = databaseName,
                [$"{AthletePhotoStorageOptions.SectionName}:Provider"] = "InMemory"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAthletePhotoStorage>();
            services.AddSingleton<IAthletePhotoStorage, InMemoryAthletePhotoStorage>();
        });
    }
}



