using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Infrastructure.Storage;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.Validation;
using BodyMetricsApi.Shared.ViewModels;
using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.GetAll;

public sealed class GetAllAthletesQueryHandler(
    IAthleteRepository athleteRepository,
    IAthletePhotoStorage photoStorage,
    ICurrentUserService currentUserService,
    IValidator<GetAllAthletesQuery> validator)
{
    public async Task<OperationResult<PagedResponseViewModel<AthleteViewModel>>> HandleAsync(GetAllAthletesQuery query, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationResult<PagedResponseViewModel<AthleteViewModel>>.Validation(validationResult.ToErrorDictionary());
        }

        var athletes = await athleteRepository.GetAllAsync(
            currentUserService.UserId,
            query.Page,
            query.PageSize,
            query.FullName,
            query.SportId,
            query.Sector,
            query.Category,
            query.Phase,
            cancellationToken);

        var viewModels = new List<AthleteViewModel>(athletes.Items.Count);

        foreach (var athlete in athletes.Items)
        {
            viewModels.Add(await athlete.ToViewModelAsync(photoStorage, cancellationToken));
        }

        var totalPages = athletes.TotalCount == 0
            ? 0
            : (int)Math.Ceiling(athletes.TotalCount / (double)query.PageSize);

        return OperationResult<PagedResponseViewModel<AthleteViewModel>>.Success(
            new PagedResponseViewModel<AthleteViewModel>(viewModels, query.Page, query.PageSize, athletes.TotalCount, totalPages));
    }
}
