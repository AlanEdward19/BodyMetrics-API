namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Commands;

public sealed record PhysicalAssessmentCommand(
    DateOnly AssessmentDate,
    GeneralMeasurementsCommand GeneralMeasurements,
    SkinfoldsCommand? Skinfolds,
    CircumferencesCommand? Circumferences);

