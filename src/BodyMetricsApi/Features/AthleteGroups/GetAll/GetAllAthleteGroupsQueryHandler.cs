using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;

namespace BodyMetricsApi.Features.AthleteGroups.GetAll;

public sealed class GetAllAthleteGroupsQueryHandler(
    IAthleteGroupRepository groupRepository,
    ICurrentUserService currentUserService)
{
    public async Task<OperationResult<List<AthleteGroupViewModel>>> HandleAsync(GetAllAthleteGroupsQuery query, CancellationToken cancellationToken)
    {
        var groups = await groupRepository.GetAllByOwnerAsync(currentUserService.UserId, cancellationToken);
        return OperationResult<List<AthleteGroupViewModel>>.Success(groups.Select(g => g.ToViewModel()).ToList());
    }
}
