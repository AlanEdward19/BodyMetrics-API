using FluentValidation;

namespace BodyMetricsApi.Features.AthleteGroups.Compare;

public sealed class CompareAthleteGroupsQueryValidator : AbstractValidator<CompareAthleteGroupsQuery>
{
    public CompareAthleteGroupsQueryValidator()
    {
        RuleFor(x => x.GroupIds)
            .NotEmpty().WithMessage("At least two group IDs are required.")
            .Must(ids => ids.Count >= 2).WithMessage("At least two group IDs are required.")
            .Must(ids => ids.All(id => !string.IsNullOrWhiteSpace(id))).WithMessage("All group IDs must be non-empty.");
    }
}
