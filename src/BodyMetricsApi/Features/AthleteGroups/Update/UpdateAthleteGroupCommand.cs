using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.AthleteGroups.Update;

public sealed record UpdateAthleteGroupCommand(string Id, string Name) : ICommand<AthleteGroupViewModel>;
