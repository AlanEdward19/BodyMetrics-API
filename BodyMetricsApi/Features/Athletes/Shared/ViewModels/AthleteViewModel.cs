using BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.ViewModels;
using BodyMetricsApi.Features.Athletes.Shared.Enums;

namespace BodyMetricsApi.Features.Athletes.Shared.ViewModels;

public sealed record AthleteViewModel(
    string Id,
    string FullName,
    string SportId,
    string SportName,
    string Sector,
    Phase Phase,
    string Category,
    Sex Sex,
    Ethnicity Ethnicity,
    DateOnly BirthDate,
    ProfilePhotoViewModel? ProfilePhoto,
    IReadOnlyList<PhysicalAssessmentViewModel> PhysicalAssessments);

