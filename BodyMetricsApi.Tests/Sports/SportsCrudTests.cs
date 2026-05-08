using System.Net;
using System.Net.Http.Json;
using BodyMetricsApi.Features.Sports;
using BodyMetricsApi.Features.Sports.Create;
using BodyMetricsApi.Features.Sports.Shared.ViewModels;
using BodyMetricsApi.Features.Sports.Update;
using BodyMetricsApi.Tests.TestInfrastructure;

namespace BodyMetricsApi.Tests.Sports;

[Collection(MongoCollectionDefinition.Name)]
public sealed class SportsCrudTests(MongoContainerFixture mongoFixture)
{
    [Theory]
    [InlineData("Volleyball")]
    [InlineData("Basketball")]
    public async Task CreateSport_ShouldReturnCreated(string sportName)
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();

        var request = new CreateSportCommand(sportName, ["Adult", "Youth"], ["A", "B"]);
        var response = await client.PostAsJsonAsync("/api/sports", request, factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<SportResponse>(factory.JsonSerializerOptions);
        Assert.NotNull(body);
        Assert.Equal(sportName, body.Name);
        Assert.Equal(["Adult", "Youth"], body.Sectors);
    }

    [Theory]
    [InlineData(new[] { "Adult", "adult" }, new[] { "Pro" })]
    [InlineData(new[] { "Adult" }, new[] { "Pro", "pro" })]
    public async Task CreateSport_ShouldRejectDuplicateOptions(string[] sectors, string[] categories)
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();

        var request = new CreateSportCommand("Judo", sectors, categories);
        var response = await client.PostAsJsonAsync("/api/sports", request, factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetSportById_ShouldReturnCreatedSport()
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();

        var created = await CreateSportAsync(client, factory, "Swimming");
        var response = await client.GetAsync($"/api/sports/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<SportResponse>(factory.JsonSerializerOptions);
        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Id);
    }

    [Fact]
    public async Task UpdateSport_ShouldReplaceValues()
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();

        var created = await CreateSportAsync(client, factory, "Rowing");
        var request = new UpdateSportCommand(string.Empty, "Indoor Rowing", ["Elite"], ["Senior"]);

        var response = await client.PutAsJsonAsync($"/api/sports/{created.Id}", request, factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<SportResponse>(factory.JsonSerializerOptions);
        Assert.NotNull(body);
        Assert.Equal("Indoor Rowing", body.Name);
        Assert.Equal(["Elite"], body.Sectors);
        Assert.Equal(["Senior"], body.Categories);
    }

    [Fact]
    public async Task DeleteSport_ShouldRemoveEntity()
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();

        var created = await CreateSportAsync(client, factory, "Tennis");
        var deleteResponse = await client.DeleteAsync($"/api/sports/{created.Id}");
        var getResponse = await client.GetAsync($"/api/sports/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetAllSports_ShouldReturnSportsSortedByName()
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();

        await CreateSportAsync(client, factory, "Zeta Sport");
        await CreateSportAsync(client, factory, "Alpha Sport");

        var response = await client.GetAsync("/api/sports");
        var body = await response.Content.ReadFromJsonAsync<List<SportResponse>>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(new[] { "Alpha Sport", "Zeta Sport" }, body.Select(item => item.Name).ToArray());
    }

    private static async Task<SportResponse> CreateSportAsync(HttpClient client, TestApplicationFactory factory, string sportName)
    {
        var request = new CreateSportCommand(sportName, ["Adult", "Youth"], ["A", "B"]);
        var response = await client.PostAsJsonAsync("/api/sports", request, factory.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SportResponse>(factory.JsonSerializerOptions))!;
    }
}


