namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.ValueObjects;

public sealed class GeneralMeasurementsValueObject
{
    public decimal WeightKg { get; private set; }

    public decimal HeightCm { get; private set; }

    public decimal SittingHeightCm { get; private set; }

    public GeneralMeasurementsValueObject()
    {
    }

    public GeneralMeasurementsValueObject(decimal weightKg, decimal heightCm, decimal sittingHeightCm)
    {
        WeightKg = EnsurePositive(weightKg, nameof(WeightKg));
        HeightCm = EnsurePositive(heightCm, nameof(HeightCm));
        SittingHeightCm = EnsurePositive(sittingHeightCm, nameof(SittingHeightCm));
    }

    private static decimal EnsurePositive(decimal value, string propertyName)
    {
        if (value <= 0)
        {
            throw new ArgumentException($"{propertyName} must be greater than zero.", propertyName);
        }

        return value;
    }
}

