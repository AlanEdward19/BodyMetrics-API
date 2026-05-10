using BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Commands;
using BodyMetricsApi.Features.Athletes.Shared.Commands;
using BodyMetricsApi.Features.Athletes.Shared.Enums;

namespace BodyMetricsApi.Features.Athletes.Shared.Interfaces;

public interface IAthleteWriteCommand
{
    string FullName { get; }
    string SportId { get; }
    string Sector { get; }
    Phase Phase { get; }
    string Category { get; }
    Sex Sex { get; }
    Ethnicity Ethnicity { get; }
    DateOnly BirthDate { get; }
    IReadOnlyList<PhysicalAssessmentCommand> PhysicalAssessments { get; }
    ProfilePhotoUploadCommand? ProfilePhoto { get; }
}
