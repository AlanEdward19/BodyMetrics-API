using BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Validators;
using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.Shared.Validators;

public abstract class AthleteWriteCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : IAthleteWriteCommand
{
    protected AthleteWriteCommandValidator()
    {
        RuleFor(command => command.FullName).NotEmpty();
        RuleFor(command => command.SportId).NotEmpty();
        RuleFor(command => command.Sector).NotEmpty();
        RuleFor(command => command.Category).NotEmpty();
        RuleFor(command => command.BirthDate)
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("BirthDate cannot be in the future.");

        RuleFor(command => command.PhysicalAssessments)
            .NotNull()
            .Must(items => items.GroupBy(item => item.AssessmentDate).All(group => group.Count() == 1))
            .WithMessage("Assessment dates must be unique.");

        RuleForEach(command => command.PhysicalAssessments).SetValidator(new PhysicalAssessmentCommandValidator());
        RuleFor(command => command.ProfilePhoto).SetValidator(new ProfilePhotoUploadCommandValidator()!);
    }
}
