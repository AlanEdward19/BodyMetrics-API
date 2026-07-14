using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;

namespace BodyMetricsApi.Features.AthleteGroups;

public static class AthleteGroupMappings
{
    public static AthleteGroupViewModel ToViewModel(this AthleteGroup group)
    {
        return new AthleteGroupViewModel(
            group.Id,
            group.Name,
            group.AthleteIds,
            group.CreatedAt,
            group.UpdatedAt);
    }
}
