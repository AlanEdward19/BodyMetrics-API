using System.Net;
using System.Net.Http.Json;
using BodyMetricsApi.Features.AthleteGroups.Create;
using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Features.AthleteGroups.Update;
using BodyMetricsApi.Tests.TestInfrastructure;

namespace BodyMetricsApi.Tests.AthleteGroups;

[Collection(MongoCollectionDefinition.Name)]
public sealed class UpdateAthleteGroupTests(MongoContainerFixture mongoFixture, AzuriteContainerFixture azuriteFixture)
    : IClassFixture<AzuriteContainerFixture>
{
    [Fact]
    public async Task UpdateGroup_ShouldRenameSuccessfully()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-update");

        var created = await CreateGroupAsync(client, factory, "Old Name");

        var response = await client.PutAsJsonAsync($"/api/athlete-groups/{created.Id}",
            new UpdateAthleteGroupCommand(created.Id, "New Name"), factory.JsonSerializerOptions);
        var body = await response.Content.ReadFromJsonAsync<AthleteGroupViewModel>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("New Name", body.Name);
    }

    [Fact]
    public async Task UpdateGroup_ShouldRejectDuplicateNameCaseInsensitive()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-update-dup");

        await CreateGroupAsync(client, factory, "Existing Group");
        var second = await CreateGroupAsync(client, factory, "Second Group");

        var response = await client.PutAsJsonAsync($"/api/athlete-groups/{second.Id}",
            new UpdateAthleteGroupCommand(second.Id, "existing group"), factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateGroup_ShouldAllowRenamingToSameName()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-update-same");

        var created = await CreateGroupAsync(client, factory, "Same Name");

        var response = await client.PutAsJsonAsync($"/api/athlete-groups/{created.Id}",
            new UpdateAthleteGroupCommand(created.Id, "Same Name"), factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateGroup_ShouldReturnNotFoundForAnotherOwner()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var ownerClient = factory.CreateAuthenticatedClient("owner-upd");
        using var strangerClient = factory.CreateAuthenticatedClient("stranger-upd");

        var created = await CreateGroupAsync(ownerClient, factory, "Owner's Group");

        var response = await strangerClient.PutAsJsonAsync($"/api/athlete-groups/{created.Id}",
            new UpdateAthleteGroupCommand(created.Id, "Hijacked"), factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateGroup_ShouldRejectEmptyName(string name)
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-update-empty");

        var created = await CreateGroupAsync(client, factory, "Valid Name");

        var response = await client.PutAsJsonAsync($"/api/athlete-groups/{created.Id}",
            new UpdateAthleteGroupCommand(created.Id, name), factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task<AthleteGroupViewModel> CreateGroupAsync(HttpClient client, TestApplicationFactory factory, string name)
    {
        var response = await client.PostAsJsonAsync("/api/athlete-groups",
            new CreateAthleteGroupCommand(name), factory.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AthleteGroupViewModel>(factory.JsonSerializerOptions))!;
    }
}
