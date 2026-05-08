using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Sports;

public sealed record GetSportByIdQuery(string Id) : IQuery<SportResponse>;

