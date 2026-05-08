using BodyMetricsApi.Features.Sports.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Sports.GetAll;

public sealed record GetAllSportsQuery() : IQuery<IReadOnlyList<SportResponse>>;

