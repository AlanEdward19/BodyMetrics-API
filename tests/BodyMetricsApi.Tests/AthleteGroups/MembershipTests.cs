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
public sealed class MembershipTests(MongoContainerFixture mongoFixture, AzuriteContainerFixture azuriteFixture)
    : IClassFixture<AzuriteContainerFixture>
{
    [Fact]
    public async Task AddMember_ShouldReturnNoContent()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-add-member");

        var sport = await CreateSportAsync(client, factory, "Soccer-add");
        var athlete = await CreateAthleteAsync(client, factory, sport.Id, "Player One");
        var group = await CreateGroupAsync(client, factory, "Starters-add");

        var response = await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task AddMember_ShouldContainAthleteIdAfterAddition()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-add-verify");

        var sport = await CreateSportAsync(client, factory, "Soccer-verify");
        var athlete = await CreateAthleteAsync(client, factory, sport.Id, "Player Verify");
        var group = await CreateGroupAsync(client, factory, "Starters-verify");

        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}", null);

        var getResponse = await client.GetAsync($"/api/athlete-groups/{group.Id}");
        var body = await getResponse.Content.ReadFromJsonAsync<AthleteGroupViewModel>(factory.JsonSerializerOptions);

        Assert.NotNull(body);
        Assert.Contains(body.Members, m => m.Id == athlete.Id);
    }

    [Fact]
    public async Task AddMember_ShouldBeIdempotent()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-add-idempotent");

        var sport = await CreateSportAsync(client, factory, "Soccer-idem");
        var athlete = await CreateAthleteAsync(client, factory, sport.Id, "Player Idem");
        var group = await CreateGroupAsync(client, factory, "Starters-idem");

        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}", null);
        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}", null);

        var getResponse = await client.GetAsync($"/api/athlete-groups/{group.Id}");
        var body = await getResponse.Content.ReadFromJsonAsync<AthleteGroupViewModel>(factory.JsonSerializerOptions);

        Assert.NotNull(body);
        Assert.Single(body.Members, m => m.Id == athlete.Id);
    }

    [Fact]
    public async Task AddMember_ShouldReturnNotFoundForAthleteOfAnotherOwner()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var ownerClient = factory.CreateAuthenticatedClient("owner-add-cross");
        using var strangerClient = factory.CreateAuthenticatedClient("stranger-add-cross");

        var sport = await CreateSportAsync(ownerClient, factory, "Soccer-cross-own");
        var ownerAthlete = await CreateAthleteAsync(ownerClient, factory, sport.Id, "Owner Athlete");
        var strangerSport = await CreateSportAsync(strangerClient, factory, "Soccer-cross-str");
        var strangerAthlete = await CreateAthleteAsync(strangerClient, factory, strangerSport.Id, "Stranger Athlete");
        var group = await CreateGroupAsync(ownerClient, factory, "My Group Cross");

        var response = await ownerClient.PostAsync($"/api/athlete-groups/{group.Id}/members/{strangerAthlete.Id}", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddMember_ShouldReturnNotFoundForGroupOfAnotherOwner()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var ownerClient = factory.CreateAuthenticatedClient("owner-add-grp-cross");
        using var strangerClient = factory.CreateAuthenticatedClient("stranger-add-grp-cross");

        var sport = await CreateSportAsync(ownerClient, factory, "Soccer-grp-cross");
        var athlete = await CreateAthleteAsync(ownerClient, factory, sport.Id, "Athlete GrpCross");
        var strangerGroup = await CreateGroupAsync(strangerClient, factory, "Stranger Group Cross");

        var response = await ownerClient.PostAsync($"/api/athlete-groups/{strangerGroup.Id}/members/{athlete.Id}", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddMember_ShouldReturnNotFoundForNonExistentAthlete()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-add-noathlete");

        var group = await CreateGroupAsync(client, factory, "Group NoAthlete");

        var response = await client.PostAsync($"/api/athlete-groups/{group.Id}/members/000000000000000000000099", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveMember_ShouldReturnNoContent()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-remove");

        var sport = await CreateSportAsync(client, factory, "Soccer-rem");
        var athlete = await CreateAthleteAsync(client, factory, sport.Id, "Player Remove");
        var group = await CreateGroupAsync(client, factory, "Group Remove");

        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}", null);

        var response = await client.DeleteAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RemoveMember_ShouldNoLongerContainAthleteIdAfterRemoval()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-remove-verify");

        var sport = await CreateSportAsync(client, factory, "Soccer-rem-verify");
        var athlete = await CreateAthleteAsync(client, factory, sport.Id, "Player RemoveV");
        var group = await CreateGroupAsync(client, factory, "Group RemoveV");

        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}", null);
        await client.DeleteAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}");

        var getResponse = await client.GetAsync($"/api/athlete-groups/{group.Id}");
        var body = await getResponse.Content.ReadFromJsonAsync<AthleteGroupViewModel>(factory.JsonSerializerOptions);

        Assert.NotNull(body);
        Assert.DoesNotContain(body.Members, m => m.Id == athlete.Id);
    }

    [Fact]
    public async Task RemoveMember_ShouldBeIdempotentForNonMember()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-remove-idem");

        var group = await CreateGroupAsync(client, factory, "Group RemoveIdem");

        var response = await client.DeleteAsync($"/api/athlete-groups/{group.Id}/members/000000000000000000000011");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RemoveMember_ShouldReturnNotFoundForGroupOfAnotherOwner()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var ownerClient = factory.CreateAuthenticatedClient("owner-rem-cross");
        using var strangerClient = factory.CreateAuthenticatedClient("stranger-rem-cross");

        var strangerGroup = await CreateGroupAsync(strangerClient, factory, "Stranger Rem Cross");

        var response = await ownerClient.DeleteAsync($"/api/athlete-groups/{strangerGroup.Id}/members/someAthleteId");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveMember_ShouldNotDeleteTheAthlete()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-rem-athlete-exists");

        var sport = await CreateSportAsync(client, factory, "Soccer-rem-exists");
        var athlete = await CreateAthleteAsync(client, factory, sport.Id, "Player StillExists");
        var group = await CreateGroupAsync(client, factory, "Group StillExists");

        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}", null);
        await client.DeleteAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}");

        var athleteResponse = await client.GetAsync($"/api/athletes/{athlete.Id}");
        Assert.Equal(HttpStatusCode.OK, athleteResponse.StatusCode);
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
