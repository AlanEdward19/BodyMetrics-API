using BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Commands;
using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Validators;

public sealed class GeneralMeasurementsCommandValidator : AbstractValidator<GeneralMeasurementsCommand>
{
    public GeneralMeasurementsCommandValidator()
    {
        RuleFor(measurements => measurements.WeightKg).GreaterThan(0);
        RuleFor(measurements => measurements.HeightCm).GreaterThan(0);
        RuleFor(measurements => measurements.SittingHeightCm).GreaterThan(0);
    }
}

