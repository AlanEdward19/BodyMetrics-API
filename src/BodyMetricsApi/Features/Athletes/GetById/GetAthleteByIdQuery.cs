using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Athletes.GetById;

public sealed record GetAthleteByIdQuery(string Id) : IQuery<AthleteViewModel>;
