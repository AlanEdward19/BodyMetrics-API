using System.Net;
using System.Net.Http.Json;
using BodyMetricsApi.Features.Athletes;
using BodyMetricsApi.Features.Athletes.Create;
using BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Commands;
using BodyMetricsApi.Features.Athletes.Shared.Commands;
using BodyMetricsApi.Features.Athletes.Shared.Enums;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Features.Athletes.Update;
using BodyMetricsApi.Features.Sports;
using BodyMetricsApi.Features.Sports.Create;
using BodyMetricsApi.Features.Sports.Shared.ViewModels;
using BodyMetricsApi.Tests.TestInfrastructure;

namespace BodyMetricsApi.Tests.Athletes;

[Collection(MongoCollectionDefinition.Name)]
public sealed class AthletesCrudTests(MongoContainerFixture mongoFixture)
{
    [Fact]
    public async Task CreateAthlete_ShouldReturnCreatedAndGeneratePhotoUrl()
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();
        var sport = await CreateSportAsync(client, factory, "Volleyball");

        var request = BuildCreateAthleteCommand(sport.Id, "Adult", "A", includePhoto: true);
        var response = await client.PostAsJsonAsync("/api/athletes", request, factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AthleteViewModel>(factory.JsonSerializerOptions);
        Assert.NotNull(body);
        Assert.Equal("Jane Doe", body.FullName);
        Assert.NotNull(body.ProfilePhoto);
        Assert.NotNull(body.ProfilePhoto!.AccessUrl);
        Assert.StartsWith("https://photos.local/", body.ProfilePhoto.AccessUrl);
    }

    [Theory]
    [InlineData("Invalid Sector", "A")]
    [InlineData("Adult", "Invalid Category")]
    public async Task CreateAthlete_ShouldRejectInvalidSportOptions(string sector, string category)
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();
        var sport = await CreateSportAsync(client, factory, "Handball");

        var request = BuildCreateAthleteCommand(sport.Id, sector, category);
        var response = await client.PostAsJsonAsync("/api/athletes", request, factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public async Task CreateAthlete_ShouldRejectFutureBirthDate(int daysInFuture)
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();
        var sport = await CreateSportAsync(client, factory, "Gymnastics");

        var request = BuildCreateAthleteCommand(sport.Id, "Adult", "A") with
        {
            BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysInFuture))
        };

        var response = await client.PostAsJsonAsync("/api/athletes", request, factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAthleteById_ShouldReturnAssessments()
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();
        var sport = await CreateSportAsync(client, factory, "Swimming");
        var created = await CreateAthleteAsync(client, factory, sport.Id);

        var response = await client.GetAsync($"/api/athletes/{created.Id}");
        var body = await response.Content.ReadFromJsonAsync<AthleteViewModel>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Single(body.PhysicalAssessments);
        Assert.Equal(70.4m, body.PhysicalAssessments[0].GeneralMeasurements.WeightKg);
    }

    [Fact]
    public async Task UpdateAthlete_ShouldReplaceAssessmentsAndAllowPhotoReplacement()
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();
        var sport = await CreateSportAsync(client, factory, "Cycling");
        var created = await CreateAthleteAsync(client, factory, sport.Id);

        var updateRequest = CreateUpdateAthleteCommand(created.Id, sport.Id, "Youth", "B", includePhoto: true) with
        {
            FullName = "Updated Athlete",
            PhysicalAssessments =
            [
                new PhysicalAssessmentCommand(
                    new DateOnly(2026, 01, 15),
                    new GeneralMeasurementsCommand(68.1m, 180.0m, 91.0m),
                    null,
                    null),
                new PhysicalAssessmentCommand(
                    new DateOnly(2026, 02, 15),
                    new GeneralMeasurementsCommand(67.5m, 180.0m, 91.0m),
                    null,
                    null)
            ]
        };

        var response = await client.PutAsJsonAsync($"/api/athletes/{created.Id}", updateRequest, factory.JsonSerializerOptions);
        var body = await response.Content.ReadFromJsonAsync<AthleteViewModel>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Updated Athlete", body.FullName);
        Assert.Equal("Youth", body.Sector);
        Assert.Equal("B", body.Category);
        Assert.Equal(2, body.PhysicalAssessments.Count);
        Assert.NotNull(body.ProfilePhoto);
    }

    [Fact]
    public async Task DeleteAthlete_ShouldRemoveEntity()
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();
        var sport = await CreateSportAsync(client, factory, "Boxing");
        var created = await CreateAthleteAsync(client, factory, sport.Id);

        var deleteResponse = await client.DeleteAsync($"/api/athletes/{created.Id}");
        var getResponse = await client.GetAsync($"/api/athletes/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetAllAthletes_ShouldReturnAthletesSortedByName()
    {
        await using var factory = new TestApplicationFactory(mongoFixture);
        using var client = factory.CreateClient();
        var sport = await CreateSportAsync(client, factory, "Track and Field");

        await CreateAthleteAsync(client, factory, sport.Id, "Zeta Runner");
        await CreateAthleteAsync(client, factory, sport.Id, "Alpha Runner");

        var response = await client.GetAsync("/api/athletes");
        var body = await response.Content.ReadFromJsonAsync<List<AthleteViewModel>>(factory.JsonSerializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(new[] { "Alpha Runner", "Zeta Runner" }, body.Select(item => item.FullName).ToArray());
    }

    private static async Task<SportResponse> CreateSportAsync(HttpClient client, TestApplicationFactory factory, string name)
    {
        var request = new CreateSportCommand(name, ["Adult", "Youth"], ["A", "B"]);
        var response = await client.PostAsJsonAsync("/api/sports", request, factory.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SportResponse>(factory.JsonSerializerOptions))!;
    }

    private static async Task<AthleteViewModel> CreateAthleteAsync(HttpClient client, TestApplicationFactory factory, string sportId, string fullName = "Jane Doe")
    {
        var request = BuildCreateAthleteCommand(sportId, "Adult", "A") with { FullName = fullName };
        var response = await client.PostAsJsonAsync("/api/athletes", request, factory.JsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AthleteViewModel>(factory.JsonSerializerOptions))!;
    }

    private static CreateAthleteCommand BuildCreateAthleteCommand(string sportId, string sector, string category, bool includePhoto = false)
    {
        return new CreateAthleteCommand(
            "Jane Doe",
            sportId,
            sector,
            Phase.Competitive,
            category,
            Sex.Female,
            Ethnicity.Asian,
            new DateOnly(1999, 04, 08),
            [
                new PhysicalAssessmentCommand(
                    new DateOnly(2026, 01, 01),
                    new GeneralMeasurementsCommand(70.4m, 177.2m, 92.5m),
                    new SkinfoldsCommand(10.0m, 10.5m, null, null, null, null, 12.1m, null, null, null, null),
                    new CircumferencesCommand(108.0m, 94.0m, null, null, 75.0m, 96.0m, null, null, 37.0m, 36.5m, null, null, null))
            ],
            includePhoto
                ? new ProfilePhotoUploadCommand("avatar.png", "image/png", Convert.ToBase64String([1, 2, 3, 4]))
                : null);
    }

    private static UpdateAthleteCommand CreateUpdateAthleteCommand(string id, string sportId, string sector, string category, bool includePhoto = false)
    {
        return new UpdateAthleteCommand(
            id,
            "Jane Doe",
            sportId,
            sector,
            Phase.Competitive,
            category,
            Sex.Female,
            Ethnicity.Asian,
            new DateOnly(1999, 04, 08),
            [
                new PhysicalAssessmentCommand(
                    new DateOnly(2026, 01, 01),
                    new GeneralMeasurementsCommand(70.4m, 177.2m, 92.5m),
                    new SkinfoldsCommand(10.0m, 10.5m, null, null, null, null, 12.1m, null, null, null, null),
                    new CircumferencesCommand(108.0m, 94.0m, null, null, 75.0m, 96.0m, null, null, 37.0m, 36.5m, null, null, null))
            ],
            includePhoto
                ? new ProfilePhotoUploadCommand("avatar.png", "image/png", Convert.ToBase64String([1, 2, 3, 4]))
                : null);
    }
}


