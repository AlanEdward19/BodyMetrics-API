using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Infrastructure.Storage;

namespace BodyMetricsApi.Features.Athletes.GetAll;

public sealed class GetAllAthletesQueryHandler(IAthleteRepository athleteRepository, IAthletePhotoStorage photoStorage)
{
    public async Task<IReadOnlyList<AthleteViewModel>> HandleAsync(GetAllAthletesQuery query, CancellationToken cancellationToken)
    {
        var athletes = await athleteRepository.GetAllAsync(cancellationToken);
        var viewModels = new List<AthleteViewModel>(athletes.Count);

        foreach (var athlete in athletes)
        {
            viewModels.Add(await athlete.ToViewModelAsync(photoStorage, cancellationToken));
        }

        return viewModels;
    }
}
