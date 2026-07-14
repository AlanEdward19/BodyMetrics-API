using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;

namespace BodyMetricsApi.Features.AthleteGroups.GetById;

public sealed class GetAthleteGroupByIdQueryHandler(
    IAthleteGroupRepository groupRepository,
    ICurrentUserService currentUserService)
{
    public async Task<OperationResult<AthleteGroupViewModel>> HandleAsync(GetAthleteGroupByIdQuery query, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(query.Id, currentUserService.UserId, cancellationToken);
        if (group is null)
        {
            return OperationResult<AthleteGroupViewModel>.NotFound("Athlete group not found.");
        }

        return OperationResult<AthleteGroupViewModel>.Success(group.ToViewModel());
    }
}
