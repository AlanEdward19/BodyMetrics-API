using FluentValidation;

namespace BodyMetricsApi.Features.AthleteGroups.Update;

public sealed class UpdateAthleteGroupCommandValidator : AbstractValidator<UpdateAthleteGroupCommand>
{
    public UpdateAthleteGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}
