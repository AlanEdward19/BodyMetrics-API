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
public sealed class DeleteAthleteGroupTests(MongoContainerFixture mongoFixture, AzuriteContainerFixture azuriteFixture)
    : IClassFixture<AzuriteContainerFixture>
{
    [Fact]
    public async Task DeleteGroup_ShouldReturnNoContent()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-del");

        var created = await CreateGroupAsync(client, factory, "To Delete");

        var response = await client.DeleteAsync($"/api/athlete-groups/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteGroup_ShouldNotBeRetrievableAfterDeletion()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-del-verify");

        var created = await CreateGroupAsync(client, factory, "Gone Soon");

        await client.DeleteAsync($"/api/athlete-groups/{created.Id}");

        var getResponse = await client.GetAsync($"/api/athlete-groups/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteGroup_ShouldNotDeleteAssociatedAthletes()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var client = factory.CreateAuthenticatedClient("user-del-athletes");

        var sport = await CreateSportAsync(client, factory, "Basketball-del");
        var athlete = await CreateAthleteAsync(client, factory, sport.Id, "Athlete To Keep");
        var group = await CreateGroupAsync(client, factory, "Temp Group");

        await client.PostAsync($"/api/athlete-groups/{group.Id}/members/{athlete.Id}", null);

        await client.DeleteAsync($"/api/athlete-groups/{group.Id}");

        var athleteResponse = await client.GetAsync($"/api/athletes/{athlete.Id}");
        Assert.Equal(HttpStatusCode.OK, athleteResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteGroup_ShouldReturnNotFoundForAnotherOwner()
    {
        await using var factory = new TestApplicationFactory(mongoFixture, azuriteFixture);
        using var ownerClient = factory.CreateAuthenticatedClient("owner-del");
        using var strangerClient = factory.CreateAuthenticatedClient("stranger-del");

        var created = await CreateGroupAsync(ownerClient, factory, "Protected Group");

        var response = await strangerClient.DeleteAsync($"/api/athlete-groups/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
