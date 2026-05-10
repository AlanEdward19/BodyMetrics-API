namespace BodyMetricsApi.Features.Athletes.Import.ViewModels;

public sealed record AthleteSpreadsheetImportViewModel(
    string SportId,
    string SportName,
    int ProcessedRows,
    int CreatedAthletes,
    int UpdatedAthletes,
    int ImportedAssessments,
    int ReplacedAssessments,
    int AddedSportSectors,
    int AddedSportCategories,
    bool SportCreated);

