using BodyMetricsApi.Features.Sports.Shared.Interfaces;
using BodyMetricsApi.Features.Sports.Shared.ViewModels;
using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.Validation;
using BodyMetricsApi.Shared.ViewModels;
using FluentValidation;

namespace BodyMetricsApi.Features.Sports.GetAll;

public sealed class GetAllSportsQueryHandler(ISportRepository repository, IValidator<GetAllSportsQuery> validator)
{
    public async Task<OperationResult<PagedResponseViewModel<SportResponse>>> HandleAsync(GetAllSportsQuery query, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationResult<PagedResponseViewModel<SportResponse>>.Validation(validationResult.ToErrorDictionary());
        }

        var sports = await repository.GetAllAsync(query.Page, query.PageSize, query.Name, query.Sector, query.Category, cancellationToken);
        var totalPages = sports.TotalCount == 0
            ? 0
            : (int)Math.Ceiling(sports.TotalCount / (double)query.PageSize);

        return OperationResult<PagedResponseViewModel<SportResponse>>.Success(
            new PagedResponseViewModel<SportResponse>(
                sports.Items.Select(sport => sport.ToResponse()).ToList(),
                query.Page,
                query.PageSize,
                sports.TotalCount,
                totalPages));
    }
}

