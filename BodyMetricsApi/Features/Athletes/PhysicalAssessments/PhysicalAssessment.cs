using BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.ValueObjects;

namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments;

public sealed class PhysicalAssessment
{
    public DateOnly AssessmentDate { get; private set; }

    public GeneralMeasurementsValueObject GeneralMeasurements { get; private set; } = new();

    public SkinfoldsValueObject Skinfolds { get; private set; } = new();

    public CircumferencesValueObject Circumferences { get; private set; } = new();

    private PhysicalAssessment()
    {
    }

    public PhysicalAssessment(DateOnly assessmentDate, GeneralMeasurementsValueObject generalMeasurements, SkinfoldsValueObject skinfolds, CircumferencesValueObject circumferences)
    {
        AssessmentDate = assessmentDate;
        GeneralMeasurements = generalMeasurements ?? throw new ArgumentNullException(nameof(generalMeasurements));
        Skinfolds = skinfolds;
        Circumferences = circumferences;
    }
}