namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.ValueObjects;

public sealed class SkinfoldsValueObject
{
    public decimal? RightTricepsMm { get; private set; }
    public decimal? LeftTricepsMm { get; private set; }
    public decimal? SubscapularMm { get; private set; }
    public decimal? ThoraxMm { get; private set; }
    public decimal? SubaxillaryMm { get; private set; }
    public decimal? SuprailiacMm { get; private set; }
    public decimal? AbdominalMm { get; private set; }
    public decimal? RightThighMm { get; private set; }
    public decimal? LeftThighMm { get; private set; }
    public decimal? RightCalfMm { get; private set; }
    public decimal? LeftCalfMm { get; private set; }

    public SkinfoldsValueObject()
    {
    }

    public SkinfoldsValueObject(
        decimal? rightTricepsMm,
        decimal? leftTricepsMm,
        decimal? subscapularMm,
        decimal? thoraxMm,
        decimal? subaxillaryMm,
        decimal? suprailiacMm,
        decimal? abdominalMm,
        decimal? rightThighMm,
        decimal? leftThighMm,
        decimal? rightCalfMm,
        decimal? leftCalfMm)
    {
        RightTricepsMm = EnsurePositiveIfPresent(rightTricepsMm, nameof(RightTricepsMm));
        LeftTricepsMm = EnsurePositiveIfPresent(leftTricepsMm, nameof(LeftTricepsMm));
        SubscapularMm = EnsurePositiveIfPresent(subscapularMm, nameof(SubscapularMm));
        ThoraxMm = EnsurePositiveIfPresent(thoraxMm, nameof(ThoraxMm));
        SubaxillaryMm = EnsurePositiveIfPresent(subaxillaryMm, nameof(SubaxillaryMm));
        SuprailiacMm = EnsurePositiveIfPresent(suprailiacMm, nameof(SuprailiacMm));
        AbdominalMm = EnsurePositiveIfPresent(abdominalMm, nameof(AbdominalMm));
        RightThighMm = EnsurePositiveIfPresent(rightThighMm, nameof(RightThighMm));
        LeftThighMm = EnsurePositiveIfPresent(leftThighMm, nameof(LeftThighMm));
        RightCalfMm = EnsurePositiveIfPresent(rightCalfMm, nameof(RightCalfMm));
        LeftCalfMm = EnsurePositiveIfPresent(leftCalfMm, nameof(LeftCalfMm));
    }

    private static decimal? EnsurePositiveIfPresent(decimal? value, string propertyName)
    {
        if (value is not null && value <= 0)
        {
            throw new ArgumentException($"{propertyName} must be greater than zero when provided.", propertyName);
        }

        return value;
    }
}

