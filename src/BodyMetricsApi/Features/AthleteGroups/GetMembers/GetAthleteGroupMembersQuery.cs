using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.AthleteGroups.GetMembers;

public sealed record GetAthleteGroupMembersQuery(string GroupId) : IQuery<List<AthleteViewModel>>;
