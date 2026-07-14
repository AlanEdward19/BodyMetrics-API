using System.Net;
using System.Net.Http.Json;
using BodyMetricsApi.Features.AthleteGroups.Create;
using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Tests.TestInfrastructure;

namespace BodyMetricsApi.Tests.AthleteGroups;

[Collection(MongoCollectionDefinition.Name)]
public sealed class CreateAthleteGroupTests(MongoContainerFixture mongoFixture, AzuriteContainerFixture azuriteFixture)
    : IClassFixture<AzuriteContainerFixture>
{
    [Fact]
    public async Task CreateGroup_ShouldRequireAuthentication()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/athlete-groups",
            new CreateAthleteGroupCommand("Starters"), factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateGroup_ShouldReturnCreatedWithViewModel()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("/api/athlete-groups",
            new CreateAthleteGroupCommand("Starters"), factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AthleteGroupViewModel>(factory.JsonSerializerOptions);
        Assert.NotNull(body);
        Assert.NotEmpty(body.Id);
        Assert.Equal("Starters", body.Name);
        Assert.Empty(body.AthleteIds);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateGroup_ShouldRejectEmptyName(string name)
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("/api/athlete-groups",
            new CreateAthleteGroupCommand(name), factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateGroup_ShouldRejectDuplicateNameCaseInsensitive()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient();

        await client.PostAsJsonAsync("/api/athlete-groups",
            new CreateAthleteGroupCommand("Reserves"), factory.JsonSerializerOptions);

        var response = await client.PostAsJsonAsync("/api/athlete-groups",
            new CreateAthleteGroupCommand("reserves"), factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateGroup_ShouldAllowSameNameForDifferentOwners()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var clientA = factory.CreateAuthenticatedClient("owner-a");
        using var clientB = factory.CreateAuthenticatedClient("owner-b");

        var responseA = await clientA.PostAsJsonAsync("/api/athlete-groups",
            new CreateAthleteGroupCommand("Team Alpha"), factory.JsonSerializerOptions);
        var responseB = await clientB.PostAsJsonAsync("/api/athlete-groups",
            new CreateAthleteGroupCommand("Team Alpha"), factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.Created, responseA.StatusCode);
        Assert.Equal(HttpStatusCode.Created, responseB.StatusCode);
    }
}
