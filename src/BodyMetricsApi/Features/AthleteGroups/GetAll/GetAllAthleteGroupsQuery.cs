using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.AthleteGroups.GetAll;

public sealed record GetAllAthleteGroupsQuery : IQuery<List<AthleteGroupViewModel>>;
