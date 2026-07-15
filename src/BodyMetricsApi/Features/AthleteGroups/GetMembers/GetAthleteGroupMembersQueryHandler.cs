using BodyMetricsApi.Features.Athletes;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Infrastructure.Storage;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;

namespace BodyMetricsApi.Features.AthleteGroups.GetMembers;

public sealed class GetAthleteGroupMembersQueryHandler(
    IAthleteGroupRepository groupRepository,
    IAthletePhotoStorage photoStorage,
    ICurrentUserService currentUserService)
{
    public async Task<OperationResult<List<AthleteViewModel>>> HandleAsync(GetAthleteGroupMembersQuery query, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(query.GroupId, currentUserService.UserId, cancellationToken);
        if (group is null)
        {
            return OperationResult<List<AthleteViewModel>>.NotFound("Athlete group not found.");
        }

        var viewModels = new List<AthleteViewModel>(group.Members.Count);
        foreach (var athlete in group.Members)
        {
            viewModels.Add(await athlete.ToViewModelAsync(photoStorage, cancellationToken));
        }

        return OperationResult<List<AthleteViewModel>>.Success(viewModels);
    }
}
