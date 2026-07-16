using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;

namespace BodyMetricsApi.Features.AthleteGroups;

public static class AthleteGroupMappings
{
    public static AthleteGroupViewModel ToViewModel(this AthleteGroup group)
    {
        return new AthleteGroupViewModel(
            group.Id,
            group.Name,
            group.Members
                .Select(member => new AthleteGroupMemberViewModel(
                    member.Id,
                    member.FullName,
                    member.SportName,
                    member.Category,
                    member.Sector))
                .ToList(),
            group.CreatedAt,
            group.UpdatedAt);
    }
}
