using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Athletes.Delete;

public sealed record DeleteAthleteCommand(string Id) : ICommand;

