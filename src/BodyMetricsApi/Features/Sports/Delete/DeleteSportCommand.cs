using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Sports.Delete;

public sealed record DeleteSportCommand(string Id) : ICommand;

