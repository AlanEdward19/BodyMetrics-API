using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.AthleteGroups;
using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;

namespace BodyMetricsApi.Features.Athletes.Shared;

// An athlete physically lives in exactly one place: the standalone Athletes collection,
// or embedded inside one AthleteGroup's Members list. This centralizes "find it wherever
// it is" and "write it back to wherever it came from" so callers don't need to know which.
public sealed record AthleteLocation(Athlete Athlete, AthleteGroup? Group);

public sealed class AthleteLocator(IAthleteRepository athleteRepository, IAthleteGroupRepository groupRepository)
{
    public async Task<AthleteLocation?> FindAsync(string athleteId, string ownerUserId, CancellationToken cancellationToken)
    {
        var standalone = await athleteRepository.GetByIdAsync(athleteId, ownerUserId, cancellationToken);
        if (standalone is not null)
        {
            return new AthleteLocation(standalone, null);
        }

        var group = await groupRepository.FindByMemberIdAsync(ownerUserId, athleteId, cancellationToken);
        var member = group?.Members.FirstOrDefault(m => m.Id == athleteId);
        return member is null ? null : new AthleteLocation(member, group);
    }

    public async Task SaveAsync(AthleteLocation location, CancellationToken cancellationToken)
    {
        if (location.Group is null)
        {
            await athleteRepository.ReplaceAsync(location.Athlete, cancellationToken);
            return;
        }

        location.Group.ReplaceMember(location.Athlete);
        await groupRepository.UpdateAsync(location.Group, cancellationToken);
    }

    public async Task DetachAsync(AthleteLocation location, CancellationToken cancellationToken)
    {
        if (location.Group is null)
        {
            await athleteRepository.DeleteAsync(location.Athlete.Id, location.Athlete.OwnerUserId, cancellationToken);
            return;
        }

        location.Group.RemoveMember(location.Athlete.Id);
        await groupRepository.UpdateAsync(location.Group, cancellationToken);
    }
}
