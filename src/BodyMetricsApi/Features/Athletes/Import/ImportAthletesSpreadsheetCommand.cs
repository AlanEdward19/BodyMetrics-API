using BodyMetricsApi.Features.Athletes.Import.ViewModels;
using BodyMetricsApi.Shared.CQRS;
using Microsoft.AspNetCore.Http;

namespace BodyMetricsApi.Features.Athletes.Import;

public sealed class ImportAthletesSpreadsheetCommand : ICommand<AthleteSpreadsheetImportViewModel>
{
    public string SportName { get; init; } = string.Empty;

    public IFormFile? File { get; init; }
}

