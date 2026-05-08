using System.Text.Json;
using System.Text.Json.Serialization;
using BodyMetricsApi.Infrastructure.Configuration;
using BodyMetricsApi.Infrastructure.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BodyMetricsApi.Tests.TestInfrastructure;

public sealed class TestApplicationFactory(MongoContainerFixture mongoFixture, AzuriteContainerFixture azuriteFixture) : WebApplicationFactory<Program>
{
    public JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(), new DateOnlyJsonConverter() }
    };

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var databaseName = $"bodymetrics-tests-{Guid.NewGuid():N}";

        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{MongoDbOptions.SectionName}:ConnectionString"] = mongoFixture.ConnectionString,
                [$"{MongoDbOptions.SectionName}:DatabaseName"] = databaseName,
                [$"{AthletePhotoStorageOptions.SectionName}:Provider"] = "AzureBlob",
                [$"{AthletePhotoStorageOptions.SectionName}:ConnectionString"] = azuriteFixture.ConnectionString,
                [$"{AthletePhotoStorageOptions.SectionName}:ContainerName"] = $"athlete-photos-tests-{Guid.NewGuid():N}",
                [$"{FirebaseAuthenticationOptions.SectionName}:ProjectId"] = "bodymetrics-tests"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.SchemeName, _ => { });
        });
    }

    public HttpClient CreateAuthenticatedClient(string userId = "test-user-1")
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.UserIdHeaderName, userId);
        return client;
    }
}



