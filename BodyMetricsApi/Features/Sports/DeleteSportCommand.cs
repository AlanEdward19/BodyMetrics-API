using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Sports;

public sealed record DeleteSportCommand(string Id) : ICommand;

