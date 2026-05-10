using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Infrastructure.Storage;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;

namespace BodyMetricsApi.Features.Athletes.GetById;

public sealed class GetAthleteByIdQueryHandler(
    IAthleteRepository athleteRepository,
    IAthletePhotoStorage photoStorage,
    ICurrentUserService currentUserService)
{
    public async Task<OperationResult<AthleteViewModel>> HandleAsync(GetAthleteByIdQuery query, CancellationToken cancellationToken)
    {
        var athlete = await athleteRepository.GetByIdAsync(query.Id, currentUserService.UserId, cancellationToken);
        if (athlete is null)
        {
            return OperationResult<AthleteViewModel>.NotFound($"Athlete '{query.Id}' was not found.");
        }

        var viewModel = await athlete.ToViewModelAsync(photoStorage, cancellationToken);
        return OperationResult<AthleteViewModel>.Success(viewModel);
    }
}
