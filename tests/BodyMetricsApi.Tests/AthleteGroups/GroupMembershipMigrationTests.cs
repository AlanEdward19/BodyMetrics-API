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
using BodyMetricsApi.Shared.ViewModels;
using BodyMetricsApi.Tests.TestInfrastructure;

namespace BodyMetricsApi.Tests.AthleteGroups;

[Collection(MongoCollectionDefinition.Name)]
public sealed class GroupMembershipMigrationTests(MongoContainerFixture mongoFixture, AzuriteContainerFixture azuriteFixture)
    : IClassFixture<AzuriteContainerFixture>
{
    [Fact]
    public async Task GetMembers_ShouldReturnEmbeddedAthletes()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-getmembers");

        var sport = await CreateSportAsync(client, factory, "Soccer-getmembers");
        var athlete = await CreateAthleteAsync(client, factory, sport.Id, "Members Athlete");
        var group = await CreateGroupAsync(client, factory, "Members Group");

        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}", null);

        var response = await client.GetAsync($"/api/athlete-groups/{group.Id}/members");
        var body = await response.Content.ReadFromJsonAsync<List<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Single(body);
        Assert.Equal(athlete.Id, body[0].Id);
    }

    [Fact]
    public async Task AddMember_ToDifferentGroup_ShouldMoveAthleteBetweenGroups()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-move-group");

        var sport = await CreateSportAsync(client, factory, "Soccer-move");
        var athlete = await CreateAthleteAsync(client, factory, sport.Id, "Move Athlete");
        var groupA = await CreateGroupAsync(client, factory, "Group Move A");
        var groupB = await CreateGroupAsync(client, factory, "Group Move B");

        await client.PostAsync($"/api/athlete-groups/{groupA.Id}/members/{athlete.Id}", null);
        var moveResponse = await client.PostAsync($"/api/athlete-groups/{groupB.Id}/members/{athlete.Id}", null);

        Assert.Equal(HttpStatusCode.NoContent, moveResponse.StatusCode);

        var groupAMembers = await (await client.GetAsync($"/api/athlete-groups/{groupA.Id}/members"))
            .Content.ReadFromJsonAsync<List<AthleteViewModel>>(factory.JsonSerializerOptions);
        var groupBMembers = await (await client.GetAsync($"/api/athlete-groups/{groupB.Id}/members"))
            .Content.ReadFromJsonAsync<List<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.NotNull(groupAMembers);
        Assert.NotNull(groupBMembers);
        Assert.Empty(groupAMembers);
        Assert.Single(groupBMembers, a => a.Id == athlete.Id);
    }

    [Fact]
    public async Task DefaultAthleteList_ShouldExcludeGroupedAthletes()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-default-excl");

        var sport = await CreateSportAsync(client, factory, "Soccer-default-excl");
        var ungrouped = await CreateAthleteAsync(client, factory, sport.Id, "Ungrouped Athlete");
        var grouped = await CreateAthleteAsync(client, factory, sport.Id, "Grouped Athlete");
        var group = await CreateGroupAsync(client, factory, "Excl Group");

        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{grouped.Id}", null);

        var response = await client.GetAsync("/api/athletes?page=1&pageSize=20");
        var body = await response.Content.ReadFromJsonAsync<PagedResponseViewModel<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.NotNull(body);
        Assert.Equal(1, body.TotalCount);
        Assert.Equal(ungrouped.Id, body.Items[0].Id);
    }

    [Fact]
    public async Task IncludeGrouped_ShouldReturnBothUngroupedAndGroupedAthletes()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-include-grouped");

        var sport = await CreateSportAsync(client, factory, "Soccer-include-grouped");
        var ungrouped = await CreateAthleteAsync(client, factory, sport.Id, "Ungrouped Incl Athlete");
        var grouped = await CreateAthleteAsync(client, factory, sport.Id, "Grouped Incl Athlete");
        var group = await CreateGroupAsync(client, factory, "Include Group");

        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{grouped.Id}", null);

        var response = await client.GetAsync("/api/athletes?page=1&pageSize=20&includeGrouped=true");
        var body = await response.Content.ReadFromJsonAsync<PagedResponseViewModel<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.NotNull(body);
        Assert.Equal(2, body.TotalCount);
        Assert.Contains(body.Items, a => a.Id == ungrouped.Id);
        Assert.Contains(body.Items, a => a.Id == grouped.Id);
    }

    [Fact]
    public async Task DeleteGroup_ShouldReturnMembersToDefaultAthleteList()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-delete-migrate");

        var sport = await CreateSportAsync(client, factory, "Soccer-delete-migrate");
        var athlete = await CreateAthleteAsync(client, factory, sport.Id, "Delete Migrate Athlete");
        var group = await CreateGroupAsync(client, factory, "Delete Migrate Group");

        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}", null);
        await client.DeleteAsync($"/api/athlete-groups/{group.Id}");

        var response = await client.GetAsync("/api/athletes?page=1&pageSize=20");
        var body = await response.Content.ReadFromJsonAsync<PagedResponseViewModel<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.NotNull(body);
        Assert.Equal(1, body.TotalCount);
        Assert.Equal(athlete.Id, body.Items[0].Id);
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
        var response = await client.PostAsJsonAsync("/api/sports",
            new CreateSportCommand(name, ["Adult"], ["A"]), factory.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SportResponse>(factory.JsonSerializerOptions))!;
    }

    private static async Task<AthleteViewModel> CreateAthleteAsync(HttpClient client, TestApplicationFactory factory, string sportId, string fullName = "Test Athlete")
    {
        var cmd = new CreateAthleteCommand(
            fullName, sportId, "Adult", Phase.Competitive, "A", Sex.Male, Ethnicity.White,
            new DateOnly(1995, 1, 1),
            [new PhysicalAssessmentCommand(new DateOnly(2026, 1, 1),
                new GeneralMeasurementsCommand(75m, 180m, 92m), null, null)], null);
        var response = await client.PostAsJsonAsync("/api/athletes", cmd, factory.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AthleteViewModel>(factory.JsonSerializerOptions))!;
    }
}
