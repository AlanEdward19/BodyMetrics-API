using FluentValidation;

namespace BodyMetricsApi.Features.Sports.GetAll;

public sealed class GetAllSportsQueryValidator : AbstractValidator<GetAllSportsQuery>
{
    public GetAllSportsQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
    }
}

