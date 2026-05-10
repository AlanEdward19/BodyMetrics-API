using BodyMetricsApi.Features.Sports.Shared.Interfaces;
using FluentValidation;

namespace BodyMetricsApi.Features.Sports.Shared.Validators;

/// <summary>
/// Base validator for Sport write commands (Create/Update).
/// Enforces common validation rules for all Sport write operations.
/// </summary>
public abstract class SportWriteCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : ISportWriteCommand
{
    protected SportWriteCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Sport name is required.");

        RuleFor(command => command.Sectors)
            .NotNull()
            .Must(sectors => sectors.Count > 0)
            .WithMessage("At least one sector is required.")
            .Must(sectors => sectors.Distinct(StringComparer.OrdinalIgnoreCase).Count() == sectors.Count)
            .WithMessage("Sector values must be unique.");

        RuleFor(command => command.Categories)
            .NotNull()
            .Must(categories => categories.Count > 0)
            .WithMessage("At least one category is required.")
            .Must(categories => categories.Distinct(StringComparer.OrdinalIgnoreCase).Count() == categories.Count)
            .WithMessage("Category values must be unique.");
    }
}

