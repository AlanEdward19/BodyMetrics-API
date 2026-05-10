using BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Commands;
using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Validators;

public sealed class SkinfoldsCommandValidator : AbstractValidator<SkinfoldsCommand>
{
    public SkinfoldsCommandValidator()
    {
        RuleFor(skinfolds => skinfolds.RightTricepsMm).GreaterThan(0).When(value => value.RightTricepsMm.HasValue);
        RuleFor(skinfolds => skinfolds.LeftTricepsMm).GreaterThan(0).When(value => value.LeftTricepsMm.HasValue);
        RuleFor(skinfolds => skinfolds.SubscapularMm).GreaterThan(0).When(value => value.SubscapularMm.HasValue);
        RuleFor(skinfolds => skinfolds.ThoraxMm).GreaterThan(0).When(value => value.ThoraxMm.HasValue);
        RuleFor(skinfolds => skinfolds.SubaxillaryMm).GreaterThan(0).When(value => value.SubaxillaryMm.HasValue);
        RuleFor(skinfolds => skinfolds.SuprailiacMm).GreaterThan(0).When(value => value.SuprailiacMm.HasValue);
        RuleFor(skinfolds => skinfolds.AbdominalMm).GreaterThan(0).When(value => value.AbdominalMm.HasValue);
        RuleFor(skinfolds => skinfolds.RightThighMm).GreaterThan(0).When(value => value.RightThighMm.HasValue);
        RuleFor(skinfolds => skinfolds.LeftThighMm).GreaterThan(0).When(value => value.LeftThighMm.HasValue);
        RuleFor(skinfolds => skinfolds.RightCalfMm).GreaterThan(0).When(value => value.RightCalfMm.HasValue);
        RuleFor(skinfolds => skinfolds.LeftCalfMm).GreaterThan(0).When(value => value.LeftCalfMm.HasValue);
    }
}

