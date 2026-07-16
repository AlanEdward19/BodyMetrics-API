using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.AthleteGroups.Delete;

public sealed record DeleteAthleteGroupCommand(string Id) : ICommand;
