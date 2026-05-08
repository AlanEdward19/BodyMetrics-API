using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.Athletes.Shared.ValueObjects;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Features.Sports;
using BodyMetricsApi.Features.Sports.Shared.Interfaces;
using BodyMetricsApi.Infrastructure.Storage;
using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.Validation;
using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.Create;

public sealed class CreateAthleteCommandHandler(
    IAthleteRepository athleteRepository,
    ISportRepository sportRepository,
    IAthletePhotoStorage photoStorage,
    IValidator<CreateAthleteCommand> validator)
{
    public async Task<OperationResult<AthleteViewModel>> HandleAsync(CreateAthleteCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationResult<AthleteViewModel>.Validation(validationResult.ToErrorDictionary());
        }

        var sport = await sportRepository.GetByIdAsync(command.SportId, cancellationToken);
        if (sport is null)
        {
            return OperationResult<AthleteViewModel>.Validation(new Dictionary<string, string[]>
            {
                [nameof(command.SportId)] = ["Sport was not found."]
            });
        }

        if (!sport.SupportsSector(command.Sector))
        {
            return OperationResult<AthleteViewModel>.Validation(new Dictionary<string, string[]>
            {
                [nameof(command.Sector)] = ["Sector is not valid for the selected sport."]
            });
        }

        if (!sport.SupportsCategory(command.Category))
        {
            return OperationResult<AthleteViewModel>.Validation(new Dictionary<string, string[]>
            {
                [nameof(command.Category)] = ["Category is not valid for the selected sport."]
            });
        }

        var athlete = Athlete.Create(
            command.FullName,
            sport,
            command.Sector,
            command.Phase,
            command.Category,
            command.Sex,
            command.Ethnicity,
            command.BirthDate,
            command.PhysicalAssessments.ToDomain(),
            null);

        var storedPhoto = await photoStorage.UploadAsync(command.ProfilePhoto?.ToUpload(), athlete.Id, cancellationToken);
        if (storedPhoto is not null)
        {
            athlete.SetProfilePhoto(new ProfilePhotoReferenceValueObject(storedPhoto.BlobPath, storedPhoto.FileName, storedPhoto.ContentType, storedPhoto.UploadedAtUtc));
        }

        await athleteRepository.AddAsync(athlete, cancellationToken);
        var viewModel = await athlete.ToViewModelAsync(photoStorage, cancellationToken);
        return OperationResult<AthleteViewModel>.Success(viewModel, StatusCodes.Status201Created);
    }
}
