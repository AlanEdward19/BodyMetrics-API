namespace BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;

public sealed record AthleteGroupViewModel(
    string Id,
    string Name,
    List<AthleteGroupMemberViewModel> Members,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record AthleteGroupMemberViewModel(
    string Id,
    string FullName,
    string SportName,
    string Category,
    string Sector);
