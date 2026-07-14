using System.Net;
using System.Net.Http.Json;
using BodyMetricsApi.Features.AthleteGroups.Create;
using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Tests.TestInfrastructure;

namespace BodyMetricsApi.Tests.AthleteGroups;

[Collection(MongoCollectionDefinition.Name)]
public sealed class GetAthleteGroupTests(MongoContainerFixture mongoFixture, AzuriteContainerFixture azuriteFixture)
    : IClassFixture<AzuriteContainerFixture>
{
    [Fact]
    public async Task GetGroupById_ShouldReturnOwnedGroup()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-getbyid");

        var created = await CreateGroupAsync(client, factory, "Defenders");

        var response = await client.GetAsync($"/api/athlete-groups/{created.Id}");
        var body = await response.Content.ReadFromJsonAsync<AthleteGroupViewModel>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Id);
        Assert.Equal("Defenders", body.Name);
    }

    [Fact]
    public async Task GetGroupById_ShouldReturnNotFoundForAnotherOwner()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var ownerClient = factory.CreateAuthenticatedClient("owner-get");
        using var strangerClient = factory.CreateAuthenticatedClient("stranger-get");

        var created = await CreateGroupAsync(ownerClient, factory, "Confidential Group");

        var response = await strangerClient.GetAsync($"/api/athlete-groups/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetGroupById_ShouldReturnNotFoundForNonExistentId()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/athlete-groups/000000000000000000000001");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllGroups_ShouldReturnOnlyOwnedGroups()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var ownerClient = factory.CreateAuthenticatedClient("owner-all");
        using var otherClient = factory.CreateAuthenticatedClient("other-all");

        await CreateGroupAsync(ownerClient, factory, "Group A");
        await CreateGroupAsync(ownerClient, factory, "Group B");
        await CreateGroupAsync(otherClient, factory, "Other Group");

        var response = await ownerClient.GetAsync("/api/athlete-groups");
        var body = await response.Content.ReadFromJsonAsync<List<AthleteGroupViewModel>>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(2, body.Count);
        Assert.All(body, g => Assert.StartsWith("Group", g.Name));
    }

    [Fact]
    public async Task GetAllGroups_ShouldReturnEmptyListWhenNoneExist()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-empty-list");

        var response = await client.GetAsync("/api/athlete-groups");
        var body = await response.Content.ReadFromJsonAsync<List<AthleteGroupViewModel>>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Empty(body);
    }

    private static async Task<AthleteGroupViewModel> CreateGroupAsync(HttpClient client, TestApplicationFactory factory, string name)
    {
        var response = await client.PostAsJsonAsync("/api/athlete-groups",
            new CreateAthleteGroupCommand(name), factory.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AthleteGroupViewModel>(factory.JsonSerializerOptions))!;
    }
}
