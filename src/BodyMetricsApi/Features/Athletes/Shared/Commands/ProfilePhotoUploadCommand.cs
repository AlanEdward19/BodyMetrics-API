using BodyMetricsApi.Infrastructure.Storage;

namespace BodyMetricsApi.Features.Athletes.Shared.Commands;

public sealed record ProfilePhotoUploadCommand(string FileName, string ContentType, string Base64Content)
{
    public AthletePhotoUpload ToUpload()
    {
        return new AthletePhotoUpload(FileName, ContentType, Convert.FromBase64String(Base64Content));
    }
}

