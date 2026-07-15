using System.Net;
using System.Net.Http.Json;
using BodyMetricsApi.Features.Athletes.Create;
using BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Commands;
using BodyMetricsApi.Features.Athletes.Shared.Commands;
using BodyMetricsApi.Features.Athletes.Shared.Enums;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Features.AthleteGroups.Create;
using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Features.Sports.Create;
using BodyMetricsApi.Features.Sports.Shared.ViewModels;
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

    [Fact]
    public async Task GetAllGroups_ShouldReturnMemberSportCategoryAndSector()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-getall-member-fields");

        var sport = await CreateSportAsync(client, factory, "Volleyball-getall");
        var athlete = await CreateAthleteAsync(client, factory, sport.Id, "Player GetAll");
        var group = await CreateGroupAsync(client, factory, "GetAll Team");

        var addResponse = await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}", null);
        addResponse.EnsureSuccessStatusCode();

        var response = await client.GetAsync("/api/athlete-groups");
        var body = await response.Content.ReadFromJsonAsync<List<AthleteGroupViewModel>>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);

        var createdGroup = body.Single(g => g.Id == group.Id);
        var member = Assert.Single(createdGroup.Members);
        Assert.Equal(athlete.Id, member.Id);
        Assert.Equal(athlete.SportName, member.SportName);
        Assert.Equal(athlete.Category, member.Category);
        Assert.Equal(athlete.Sector, member.Sector);
    }

    private static async Task<AthleteGroupViewModel> CreateGroupAsync(HttpClient client, TestApplicationFactory factory, string name)
    {
        var response = await client.PostAsJsonAsync("/api/athlete-groups",
            new CreateAthleteGroupCommand(name), factory.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AthleteGroupViewModel>(factory.JsonSerializerOptions))!;
    }

    private static async Task<SportResponse> CreateSportAsync(HttpClient client, TestApplicationFactory factory, string name)
    {
        var response = await client.PostAsJsonAsync(
            "/api/sports",
            new CreateSportCommand(name, ["Adult"], ["A"]),
            factory.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SportResponse>(factory.JsonSerializerOptions))!;
    }

    private static async Task<AthleteViewModel> CreateAthleteAsync(HttpClient client, TestApplicationFactory factory, string sportId, string fullName)
    {
        var command = new CreateAthleteCommand(
            fullName,
            sportId,
            "Adult",
            Phase.Competitive,
            "A",
            Sex.Male,
            Ethnicity.White,
            new DateOnly(1995, 1, 1),
            [new PhysicalAssessmentCommand(
                new DateOnly(2026, 1, 1),
                new GeneralMeasurementsCommand(75m, 180m, 92m),
                null,
                null)],
            null);

        var response = await client.PostAsJsonAsync("/api/athletes", command, factory.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AthleteViewModel>(factory.JsonSerializerOptions))!;
    }
}
