using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.AthleteGroups.RemoveMember;

public sealed record RemoveAthleteFromGroupCommand(string GroupId, string AthleteId) : ICommand;
