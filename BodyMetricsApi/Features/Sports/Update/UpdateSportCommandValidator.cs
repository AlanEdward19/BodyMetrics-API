using FluentValidation;

namespace BodyMetricsApi.Features.Sports.Update;

public sealed class UpdateSportCommandValidator : AbstractValidator<UpdateSportCommand>
{
    public UpdateSportCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty();
        RuleFor(command => command.Sectors)
            .NotNull()
            .Must(items => items.Count > 0)
            .Must(items => items.Distinct(StringComparer.OrdinalIgnoreCase).Count() == items.Count)
            .WithMessage("Sector values must be unique.");

        RuleFor(command => command.Categories)
            .NotNull()
            .Must(items => items.Count > 0)
            .Must(items => items.Distinct(StringComparer.OrdinalIgnoreCase).Count() == items.Count)
            .WithMessage("Category values must be unique.");
    }
}

