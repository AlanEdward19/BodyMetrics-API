namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.ViewModels;

public sealed record PhysicalAssessmentViewModel(
    DateOnly AssessmentDate,
    GeneralMeasurementsViewModel GeneralMeasurements,
    SkinfoldsViewModel Skinfolds,
    CircumferencesViewModel Circumferences);

