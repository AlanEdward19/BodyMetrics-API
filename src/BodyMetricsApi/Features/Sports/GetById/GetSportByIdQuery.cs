using BodyMetricsApi.Features.Sports.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Sports.GetById;

public sealed record GetSportByIdQuery(string Id) : IQuery<SportResponse>;

