using BodyMetricsApi.Shared.CQRS;
using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;

namespace BodyMetricsApi.Features.AthleteGroups.Create;

public sealed record CreateAthleteGroupCommand(string Name) : ICommand<AthleteGroupViewModel>;
