using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Sports;

public sealed record GetAllSportsQuery() : IQuery<IReadOnlyList<SportResponse>>;

