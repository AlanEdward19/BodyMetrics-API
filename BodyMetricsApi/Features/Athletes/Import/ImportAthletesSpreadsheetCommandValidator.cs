using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.Import;

public sealed class ImportAthletesSpreadsheetCommandValidator : AbstractValidator<ImportAthletesSpreadsheetCommand>
{
    private const string SpreadsheetContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public ImportAthletesSpreadsheetCommandValidator()
    {
        RuleFor(command => command.SportName)
            .NotEmpty();

        RuleFor(command => command.File)
            .NotNull();

        When(command => command.File is not null, () =>
        {
            RuleFor(command => command.File!.Length)
                .GreaterThan(0);

            RuleFor(command => command.File!.FileName)
                .Must(fileName => string.Equals(Path.GetExtension(fileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
                .WithMessage("The uploaded file must be an .xlsx spreadsheet.");

            RuleFor(command => command.File!.ContentType)
                .Must(contentType => string.IsNullOrWhiteSpace(contentType) || string.Equals(contentType, SpreadsheetContentType, StringComparison.OrdinalIgnoreCase))
                .WithMessage("The uploaded file must use the .xlsx content type.");
        });
    }
}

