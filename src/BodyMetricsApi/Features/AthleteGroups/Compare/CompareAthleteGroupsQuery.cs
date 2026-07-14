using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.AthleteGroups.Compare;

public sealed record CompareAthleteGroupsQuery(List<string> GroupIds) : IQuery<List<AthleteGroupComparisonViewModel>>;
