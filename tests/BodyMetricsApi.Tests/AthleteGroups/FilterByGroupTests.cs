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
public sealed class FilterByGroupTests(MongoContainerFixture mongoFixture, AzuriteContainerFixture azuriteFixture)
    : IClassFixture<AzuriteContainerFixture>
{
    [Fact]
    public async Task FilterByGroup_ShouldReturnOnlyGroupMembers()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-filter-basic");

        var sport = await CreateSportAsync(client, factory, "Soccer-filter");
        var a1 = await CreateAthleteAsync(client, factory, sport.Id, "Filter Athlete A");
        var a2 = await CreateAthleteAsync(client, factory, sport.Id, "Filter Athlete B");
        await CreateAthleteAsync(client, factory, sport.Id, "Filter Athlete C");

        var group = await CreateGroupAsync(client, factory, "Sub Group Filter");
        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{a1.Id}", null);
        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{a2.Id}", null);

        var response = await client.GetAsync($"/api/athletes?page=1&pageSize=20&groupId={group.Id}");
        var body = await response.Content.ReadFromJsonAsync<PagedResponseViewModel<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(2, body.TotalCount);
        Assert.All(body.Items, item => Assert.True(item.Id == a1.Id || item.Id == a2.Id));
    }

    [Fact]
    public async Task FilterByGroup_CombinedWithSport_ShouldApplyBothFilters()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-filter-sport");

        var sport1 = await CreateSportAsync(client, factory, "Soccer-FilterSport1");
        var sport2 = await CreateSportAsync(client, factory, "Volleyball-FilterSport2");
        var a1 = await CreateAthleteAsync(client, factory, sport1.Id, "Sport Filter A1");
        var a2 = await CreateAthleteAsync(client, factory, sport2.Id, "Sport Filter A2");

        var group = await CreateGroupAsync(client, factory, "Mixed Sports Group");
        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{a1.Id}", null);
        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{a2.Id}", null);

        var response = await client.GetAsync($"/api/athletes?page=1&pageSize=20&groupId={group.Id}&sportId={sport1.Id}");
        var body = await response.Content.ReadFromJsonAsync<PagedResponseViewModel<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(1, body.TotalCount);
        Assert.Equal(a1.Id, body.Items[0].Id);
    }

    [Fact]
    public async Task FilterByGroup_CombinedWithSector_ShouldApplyBothFilters()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-filter-sector");

        var sport = await CreateSportAsync(client, factory, "Soccer-FilterSector");
        var adultAthlete = await CreateAthleteAsync(client, factory, sport.Id, "Sector Adult", sector: "Adult");
        var youthAthlete = await CreateAthleteAsync(client, factory, sport.Id, "Sector Youth", sector: "Youth");

        var group = await CreateGroupAsync(client, factory, "Sector Test Group");
        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{adultAthlete.Id}", null);
        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{youthAthlete.Id}", null);

        var response = await client.GetAsync($"/api/athletes?page=1&pageSize=20&groupId={group.Id}&sector=Adult");
        var body = await response.Content.ReadFromJsonAsync<PagedResponseViewModel<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(1, body.TotalCount);
        Assert.Equal(adultAthlete.Id, body.Items[0].Id);
    }

    [Fact]
    public async Task FilterByGroup_CombinedWithCategory_ShouldApplyBothFilters()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-filter-category");

        var sport = await CreateSportAsync(client, factory, "Soccer-FilterCat");
        var catA = await CreateAthleteAsync(client, factory, sport.Id, "Cat A Athlete", category: "A");
        var catB = await CreateAthleteAsync(client, factory, sport.Id, "Cat B Athlete", category: "B");

        var group = await CreateGroupAsync(client, factory, "Category Test Group");
        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{catA.Id}", null);
        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{catB.Id}", null);

        var response = await client.GetAsync($"/api/athletes?page=1&pageSize=20&groupId={group.Id}&category=B");
        var body = await response.Content.ReadFromJsonAsync<PagedResponseViewModel<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.NotNull(body);
        Assert.Equal(1, body.TotalCount);
        Assert.Equal(catB.Id, body.Items[0].Id);
    }

    [Fact]
    public async Task FilterByGroup_CombinedWithPhase_ShouldApplyBothFilters()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-filter-phase");

        var sport = await CreateSportAsync(client, factory, "Soccer-FilterPhase");
        var compAthlete = await CreateAthleteAsync(client, factory, sport.Id, "Phase Comp", phase: Phase.Competitive);
        var preAthlete = await CreateAthleteAsync(client, factory, sport.Id, "Phase Pre", phase: Phase.PreSeason);

        var group = await CreateGroupAsync(client, factory, "Phase Test Group");
        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{compAthlete.Id}", null);
        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{preAthlete.Id}", null);

        var response = await client.GetAsync($"/api/athletes?page=1&pageSize=20&groupId={group.Id}&phase=Competitive");
        var body = await response.Content.ReadFromJsonAsync<PagedResponseViewModel<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.NotNull(body);
        Assert.Equal(1, body.TotalCount);
        Assert.Equal(compAthlete.Id, body.Items[0].Id);
    }

    [Fact]
    public async Task FilterByGroup_WithCrossOwnerGroupId_ShouldReturnEmptyResult()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var ownerClient = factory.CreateAuthenticatedClient("owner-filter-cross");
        using var otherClient = factory.CreateAuthenticatedClient("other-filter-cross");

        var sport = await CreateSportAsync(ownerClient, factory, "Soccer-CrossFilter");
        await CreateAthleteAsync(ownerClient, factory, sport.Id, "Owner Athlete CrossF");
        var otherGroup = await CreateGroupAsync(otherClient, factory, "Other Group CrossF");

        var response = await ownerClient.GetAsync($"/api/athletes?page=1&pageSize=20&groupId={otherGroup.Id}");
        var body = await response.Content.ReadFromJsonAsync<PagedResponseViewModel<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(0, body.TotalCount);
    }

    [Fact]
    public async Task FilterByGroup_WithEmptyGroup_ShouldReturnEmptyPage()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-filter-empty-grp");

        var sport = await CreateSportAsync(client, factory, "Soccer-EmptyGroup");
        await CreateAthleteAsync(client, factory, sport.Id, "Athlete EmptyGrp");
        var group = await CreateGroupAsync(client, factory, "Empty Group Filter");

        var response = await client.GetAsync($"/api/athletes?page=1&pageSize=20&groupId={group.Id}");
        var body = await response.Content.ReadFromJsonAsync<PagedResponseViewModel<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(0, body.TotalCount);
    }

    [Fact]
    public async Task NoGroupFilter_ShouldReturnAllAthletesForOwner()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-no-filter");

        var sport = await CreateSportAsync(client, factory, "Soccer-NoFilter");
        await CreateAthleteAsync(client, factory, sport.Id, "No Filter A1");
        await CreateAthleteAsync(client, factory, sport.Id, "No Filter A2");
        await CreateAthleteAsync(client, factory, sport.Id, "No Filter A3");

        var response = await client.GetAsync("/api/athletes?page=1&pageSize=20");
        var body = await response.Content.ReadFromJsonAsync<PagedResponseViewModel<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(3, body.TotalCount);
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
            new CreateSportCommand(name, ["Adult", "Youth"], ["A", "B"]), factory.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SportResponse>(factory.JsonSerializerOptions))!;
    }

    private static async Task<AthleteViewModel> CreateAthleteAsync(
        HttpClient client, TestApplicationFactory factory, string sportId, string fullName = "Test Athlete",
        string sector = "Adult", string category = "A", Phase phase = Phase.Competitive)
    {
        var cmd = new CreateAthleteCommand(
            fullName, sportId, sector, phase, category, Sex.Male, Ethnicity.White,
            new DateOnly(1995, 1, 1),
            [new PhysicalAssessmentCommand(new DateOnly(2026, 1, 1),
                new GeneralMeasurementsCommand(75m, 180m, 92m), null, null)], null);
        var response = await client.PostAsJsonAsync("/api/athletes", cmd, factory.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AthleteViewModel>(factory.JsonSerializerOptions))!;
    }
}
