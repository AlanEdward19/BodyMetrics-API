namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.ValueObjects;

public sealed class GeneralMeasurementsValueObject
{
    public decimal? WeightKg { get; private set; }

    public decimal? HeightCm { get; private set; }

    public decimal? SittingHeightCm { get; private set; }

    public GeneralMeasurementsValueObject()
    {
    }

    public GeneralMeasurementsValueObject(decimal? weightKg, decimal? heightCm, decimal? sittingHeightCm)
    {
        WeightKg = EnsurePositiveIfPresent(weightKg, nameof(WeightKg));
        HeightCm = EnsurePositiveIfPresent(heightCm, nameof(HeightCm));
        SittingHeightCm = EnsurePositiveIfPresent(sittingHeightCm, nameof(SittingHeightCm));
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

