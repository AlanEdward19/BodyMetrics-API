using BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Commands;
using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Validators;

public sealed class CircumferencesCommandValidator : AbstractValidator<CircumferencesCommand>
{
    public CircumferencesCommandValidator()
    {
        RuleFor(value => value.ShoulderCm).GreaterThan(0).When(value => value.ShoulderCm.HasValue && value.ShoulderCm.Value != 0);
        RuleFor(value => value.ChestCm).GreaterThan(0).When(value => value.ChestCm.HasValue && value.ChestCm.Value != 0);
        RuleFor(value => value.RightArmCm).GreaterThan(0).When(value => value.RightArmCm.HasValue && value.RightArmCm.Value != 0);
        RuleFor(value => value.LeftArmCm).GreaterThan(0).When(value => value.LeftArmCm.HasValue && value.LeftArmCm.Value != 0);
        RuleFor(value => value.WaistCm).GreaterThan(0).When(value => value.WaistCm.HasValue && value.WaistCm.Value != 0);
        RuleFor(value => value.AbdominalCm).GreaterThan(0).When(value => value.AbdominalCm.HasValue && value.AbdominalCm.Value != 0);
        RuleFor(value => value.HipCm).GreaterThan(0).When(value => value.HipCm.HasValue && value.HipCm.Value != 0);
        RuleFor(value => value.RightMidThighCm).GreaterThan(0).When(value => value.RightMidThighCm.HasValue && value.RightMidThighCm.Value != 0);
        RuleFor(value => value.LeftMidThighCm).GreaterThan(0).When(value => value.LeftMidThighCm.HasValue && value.LeftMidThighCm.Value != 0);
        RuleFor(value => value.RightCalfCm).GreaterThan(0).When(value => value.RightCalfCm.HasValue && value.RightCalfCm.Value != 0);
        RuleFor(value => value.LeftCalfCm).GreaterThan(0).When(value => value.LeftCalfCm.HasValue && value.LeftCalfCm.Value != 0);
        RuleFor(value => value.RightWristCm).GreaterThan(0).When(value => value.RightWristCm.HasValue && value.RightWristCm.Value != 0);
        RuleFor(value => value.RightKneeCm).GreaterThan(0).When(value => value.RightKneeCm.HasValue && value.RightKneeCm.Value != 0);
        RuleFor(value => value.RightAnkleCm).GreaterThan(0).When(value => value.RightAnkleCm.HasValue && value.RightAnkleCm.Value != 0);
        RuleFor(value => value.EnvergaduraCm).GreaterThan(0).When(value => value.EnvergaduraCm.HasValue && value.EnvergaduraCm.Value != 0);
    }
}

