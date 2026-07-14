using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.AthleteGroups.GetById;

public sealed record GetAthleteGroupByIdQuery(string Id) : IQuery<AthleteGroupViewModel>;
