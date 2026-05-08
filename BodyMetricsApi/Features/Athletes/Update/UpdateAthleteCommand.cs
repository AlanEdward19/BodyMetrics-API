using BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.Commands;
using BodyMetricsApi.Features.Athletes.Shared.Commands;
using BodyMetricsApi.Features.Athletes.Shared.Enums;
using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Athletes.Update;

public sealed record UpdateAthleteCommand(
    string Id,
    string FullName,
    string SportId,
    string Sector,
    Phase Phase,
    string Category,
    Sex Sex,
    Ethnicity Ethnicity,
    DateOnly BirthDate,
    IReadOnlyList<PhysicalAssessmentCommand> PhysicalAssessments,
    ProfilePhotoUploadCommand? ProfilePhoto) : ICommand<AthleteViewModel>, IAthleteWriteCommand;
