namespace BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;

public sealed record AthleteGroupViewModel(
    string Id,
    string Name,
    List<string> AthleteIds,
    DateTime CreatedAt,
    DateTime UpdatedAt);
