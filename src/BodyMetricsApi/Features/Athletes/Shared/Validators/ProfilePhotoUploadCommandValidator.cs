using BodyMetricsApi.Features.Athletes.Shared.Commands;
using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.Shared.Validators;

public sealed class ProfilePhotoUploadCommandValidator : AbstractValidator<ProfilePhotoUploadCommand>
{
    public ProfilePhotoUploadCommandValidator()
    {
        RuleFor(photo => photo.FileName).NotEmpty();
        RuleFor(photo => photo.ContentType).NotEmpty();
        RuleFor(photo => photo.Base64Content)
            .NotEmpty()
            .Must(BeValidBase64)
            .WithMessage("Profile photo content must be valid base64.");
    }

    private static bool BeValidBase64(string content)
    {
        try
        {
            Convert.FromBase64String(content);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

