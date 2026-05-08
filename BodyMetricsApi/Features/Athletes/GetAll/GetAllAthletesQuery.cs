using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Athletes.GetAll;

public sealed record GetAllAthletesQuery : IQuery<IReadOnlyList<AthleteViewModel>>;
