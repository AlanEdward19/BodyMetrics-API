namespace BodyMetricsApi.Features.Athletes.Shared.ValueObjects;

public sealed class ProfilePhotoReferenceValueObject
{
    public string BlobPath { get; private set; } = string.Empty;

    public string FileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public DateTimeOffset UploadedAtUtc { get; private set; }

    private ProfilePhotoReferenceValueObject()
    {
    }

    public ProfilePhotoReferenceValueObject(string blobPath, string fileName, string contentType, DateTimeOffset uploadedAtUtc)
    {
        BlobPath = blobPath;
        FileName = fileName;
        ContentType = contentType;
        UploadedAtUtc = uploadedAtUtc;
    }
}

