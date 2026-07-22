namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.ValueObjects;

public sealed class CircumferencesValueObject
{
    public decimal? ShoulderCm { get; private set; }
    public decimal? ChestCm { get; private set; }
    public decimal? RightArmCm { get; private set; }
    public decimal? LeftArmCm { get; private set; }
    public decimal? WaistCm { get; private set; }
    public decimal? AbdominalCm { get; private set; }
    public decimal? HipCm { get; private set; }
    public decimal? RightMidThighCm { get; private set; }
    public decimal? LeftMidThighCm { get; private set; }
    public decimal? RightCalfCm { get; private set; }
    public decimal? LeftCalfCm { get; private set; }
    public decimal? RightWristCm { get; private set; }
    public decimal? RightKneeCm { get; private set; }
    public decimal? RightAnkleCm { get; private set; }
    public decimal? EnvergaduraCm { get; private set; }

    public CircumferencesValueObject()
    {
    }

    public CircumferencesValueObject(
        decimal? shoulderCm,
        decimal? chestCm,
        decimal? rightArmCm,
        decimal? leftArmCm,
        decimal? waistCm,
        decimal? abdominalCm,
        decimal? hipCm,
        decimal? rightMidThighCm,
        decimal? leftMidThighCm,
        decimal? rightCalfCm,
        decimal? leftCalfCm,
        decimal? rightWristCm,
        decimal? rightKneeCm,
        decimal? rightAnkleCm,
        decimal? envergaduraCm = null)
    {
        ShoulderCm = EnsurePositiveIfPresent(shoulderCm, nameof(ShoulderCm));
        ChestCm = EnsurePositiveIfPresent(chestCm, nameof(ChestCm));
        RightArmCm = EnsurePositiveIfPresent(rightArmCm, nameof(RightArmCm));
        LeftArmCm = EnsurePositiveIfPresent(leftArmCm, nameof(LeftArmCm));
        WaistCm = EnsurePositiveIfPresent(waistCm, nameof(WaistCm));
        AbdominalCm = EnsurePositiveIfPresent(abdominalCm, nameof(AbdominalCm));
        HipCm = EnsurePositiveIfPresent(hipCm, nameof(HipCm));
        RightMidThighCm = EnsurePositiveIfPresent(rightMidThighCm, nameof(RightMidThighCm));
        LeftMidThighCm = EnsurePositiveIfPresent(leftMidThighCm, nameof(LeftMidThighCm));
        RightCalfCm = EnsurePositiveIfPresent(rightCalfCm, nameof(RightCalfCm));
        LeftCalfCm = EnsurePositiveIfPresent(leftCalfCm, nameof(LeftCalfCm));
        RightWristCm = EnsurePositiveIfPresent(rightWristCm, nameof(RightWristCm));
        RightKneeCm = EnsurePositiveIfPresent(rightKneeCm, nameof(RightKneeCm));
        RightAnkleCm = EnsurePositiveIfPresent(rightAnkleCm, nameof(RightAnkleCm));
        EnvergaduraCm = EnsurePositiveIfPresent(envergaduraCm, nameof(EnvergaduraCm));
    }

    private static decimal? EnsurePositiveIfPresent(decimal? value, string propertyName)
    {
        if (value is null || value == 0)
        {
            return null;
        }

        if (value < 0)
        {
            throw new ArgumentException($"{propertyName} must be greater than or equal to zero when provided.", propertyName);
        }

        return value;
    }
}

