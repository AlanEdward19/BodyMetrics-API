using BodyMetricsApi.Features.Athletes.Shared;
using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Infrastructure.Storage;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.Validation;
using BodyMetricsApi.Shared.ViewModels;
using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.GetAll;

public sealed class GetAllAthletesQueryHandler(
    IAthleteRepository athleteRepository,
    IAthleteGroupRepository groupRepository,
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

        IReadOnlyList<Athlete> pagedItems;
        long totalCount;

        if (!string.IsNullOrWhiteSpace(query.GroupId))
        {
            // A group owned by another user (or a bad id) reads as "no members", matching
            // the pre-existing behavior of this filter rather than surfacing a 404 here.
            var group = await groupRepository.GetByIdAsync(query.GroupId, currentUserService.UserId, cancellationToken);
            var filtered = AthleteFilter.Apply(group?.Members ?? [], query.FullName, query.SportId, query.Sector, query.Category, query.Phase);
            pagedItems = AthleteFilter.Paginate(filtered, query.Page, query.PageSize, out var groupFilteredCount);
            totalCount = groupFilteredCount;
        }
        else if (query.IncludeGrouped)
        {
            var standalone = await athleteRepository.GetAllRawAsync(currentUserService.UserId, cancellationToken);
            var groups = await groupRepository.GetAllByOwnerAsync(currentUserService.UserId, cancellationToken);
            var combined = standalone.Concat(groups.SelectMany(g => g.Members));
            var filtered = AthleteFilter.Apply(combined, query.FullName, query.SportId, query.Sector, query.Category, query.Phase);
            pagedItems = AthleteFilter.Paginate(filtered, query.Page, query.PageSize, out var combinedFilteredCount);
            totalCount = combinedFilteredCount;
        }
        else
        {
            var paged = await athleteRepository.GetAllAsync(
                currentUserService.UserId,
                query.Page,
                query.PageSize,
                query.FullName,
                query.SportId,
                query.Sector,
                query.Category,
                query.Phase,
                cancellationToken);
            pagedItems = paged.Items;
            totalCount = paged.TotalCount;
        }

        var viewModels = new List<AthleteViewModel>(pagedItems.Count);
        foreach (var athlete in pagedItems)
        {
            viewModels.Add(await athlete.ToViewModelAsync(photoStorage, cancellationToken));
        }

        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return OperationResult<PagedResponseViewModel<AthleteViewModel>>.Success(
            new PagedResponseViewModel<AthleteViewModel>(viewModels, query.Page, query.PageSize, totalCount, totalPages));
    }
}
