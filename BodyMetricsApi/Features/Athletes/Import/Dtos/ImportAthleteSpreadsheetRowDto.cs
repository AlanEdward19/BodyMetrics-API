using BodyMetricsApi.Features.Athletes.PhysicalAssessments;
using BodyMetricsApi.Features.Athletes.Shared.Enums;

namespace BodyMetricsApi.Features.Athletes.Import.Dtos;

internal sealed record ImportAthleteSpreadsheetRowDto(
    int RowNumber,
    string FullName,
    string Sector,
    Phase Phase,
    string Category,
    Sex Sex,
    Ethnicity Ethnicity,
    DateOnly BirthDate,
    PhysicalAssessment PhysicalAssessment);

