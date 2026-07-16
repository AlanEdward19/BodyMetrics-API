using FluentValidation;

namespace BodyMetricsApi.Features.AthleteGroups.Create;

public sealed class CreateAthleteGroupCommandValidator : AbstractValidator<CreateAthleteGroupCommand>
{
    public CreateAthleteGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}
