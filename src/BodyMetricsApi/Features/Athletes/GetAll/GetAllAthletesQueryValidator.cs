using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.GetAll;

public sealed class GetAllAthletesQueryValidator : AbstractValidator<GetAllAthletesQuery>
{
    public GetAllAthletesQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 1000);
    }
}

