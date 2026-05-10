using BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Commands;
using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Validators;

public sealed class CircumferencesCommandValidator : AbstractValidator<CircumferencesCommand>
{
    public CircumferencesCommandValidator()
    {
        RuleFor(value => value.ShoulderCm).GreaterThan(0).When(value => value.ShoulderCm.HasValue);
        RuleFor(value => value.ChestCm).GreaterThan(0).When(value => value.ChestCm.HasValue);
        RuleFor(value => value.RightArmCm).GreaterThan(0).When(value => value.RightArmCm.HasValue);
        RuleFor(value => value.LeftArmCm).GreaterThan(0).When(value => value.LeftArmCm.HasValue);
        RuleFor(value => value.WaistCm).GreaterThan(0).When(value => value.WaistCm.HasValue);
        RuleFor(value => value.HipCm).GreaterThan(0).When(value => value.HipCm.HasValue);
        RuleFor(value => value.RightMidThighCm).GreaterThan(0).When(value => value.RightMidThighCm.HasValue);
        RuleFor(value => value.LeftMidThighCm).GreaterThan(0).When(value => value.LeftMidThighCm.HasValue);
        RuleFor(value => value.RightCalfCm).GreaterThan(0).When(value => value.RightCalfCm.HasValue);
        RuleFor(value => value.LeftCalfCm).GreaterThan(0).When(value => value.LeftCalfCm.HasValue);
        RuleFor(value => value.RightWristCm).GreaterThan(0).When(value => value.RightWristCm.HasValue);
        RuleFor(value => value.RightKneeCm).GreaterThan(0).When(value => value.RightKneeCm.HasValue);
        RuleFor(value => value.RightAnkleCm).GreaterThan(0).When(value => value.RightAnkleCm.HasValue);
    }
}

