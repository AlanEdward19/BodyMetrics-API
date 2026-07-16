using BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Commands;
using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Validators;

public sealed class PhysicalAssessmentCommandValidator : AbstractValidator<PhysicalAssessmentCommand>
{
    public PhysicalAssessmentCommandValidator()
    {
        RuleFor(assessment => assessment.GeneralMeasurements).SetValidator(new GeneralMeasurementsCommandValidator()!);
        RuleFor(assessment => assessment.Skinfolds).SetValidator(new SkinfoldsCommandValidator()!);
        RuleFor(assessment => assessment.Circumferences).SetValidator(new CircumferencesCommandValidator()!);
    }
}

