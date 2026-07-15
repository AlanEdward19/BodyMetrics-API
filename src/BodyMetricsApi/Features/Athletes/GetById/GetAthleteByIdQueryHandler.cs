using BodyMetricsApi.Features.Athletes.Shared;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Infrastructure.Storage;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;

namespace BodyMetricsApi.Features.Athletes.GetById;

public sealed class GetAthleteByIdQueryHandler(
    AthleteLocator athleteLocator,
    IAthletePhotoStorage photoStorage,
    ICurrentUserService currentUserService)
{
    public async Task<OperationResult<AthleteViewModel>> HandleAsync(GetAthleteByIdQuery query, CancellationToken cancellationToken)
    {
        var location = await athleteLocator.FindAsync(query.Id, currentUserService.UserId, cancellationToken);
        if (location is null)
        {
            return OperationResult<AthleteViewModel>.NotFound($"Athlete '{query.Id}' was not found.");
        }

        var viewModel = await location.Athlete.ToViewModelAsync(photoStorage, cancellationToken);
        return OperationResult<AthleteViewModel>.Success(viewModel);
    }
}
