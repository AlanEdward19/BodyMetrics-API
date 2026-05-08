using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.Athletes.Shared.ValueObjects;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Features.Sports;
using BodyMetricsApi.Features.Sports.Shared.Interfaces;
using BodyMetricsApi.Infrastructure.Storage;
using BodyMetricsApi.Shared.Authentication;
using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.Validation;
using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.Update;

public sealed class UpdateAthleteCommandHandler(
    IAthleteRepository athleteRepository,
    ISportRepository sportRepository,
    IAthletePhotoStorage photoStorage,
    ICurrentUserService currentUserService,
    IValidator<UpdateAthleteCommand> validator)
{
    public async Task<OperationResult<AthleteViewModel>> HandleAsync(UpdateAthleteCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationResult<AthleteViewModel>.Validation(validationResult.ToErrorDictionary());
        }

        var athlete = await athleteRepository.GetByIdAsync(command.Id, currentUserService.UserId, cancellationToken);
        if (athlete is null)
        {
            return OperationResult<AthleteViewModel>.NotFound($"Athlete '{command.Id}' was not found.");
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

        ProfilePhotoReferenceValueObject? profilePhoto = athlete.ProfilePhoto;
        if (command.ProfilePhoto is not null)
        {
            var storedPhoto = await photoStorage.UploadAsync(command.ProfilePhoto.ToUpload(), command.Id, cancellationToken);
            profilePhoto = storedPhoto is null
                ? profilePhoto
                : new ProfilePhotoReferenceValueObject(storedPhoto.BlobPath, storedPhoto.FileName, storedPhoto.ContentType, storedPhoto.UploadedAtUtc);
        }

        athlete.Update(
            command.FullName,
            sport,
            command.Sector,
            command.Phase,
            command.Category,
            command.Sex,
            command.Ethnicity,
            command.BirthDate,
            command.PhysicalAssessments.ToDomain(),
            profilePhoto);

        await athleteRepository.ReplaceAsync(athlete, cancellationToken);
        var viewModel = await athlete.ToViewModelAsync(photoStorage, cancellationToken);
        return OperationResult<AthleteViewModel>.Success(viewModel);
    }
}
