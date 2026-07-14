using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.AthleteGroups.AddMember;

public sealed record AddAthleteToGroupCommand(string GroupId, string AthleteId) : ICommand;
