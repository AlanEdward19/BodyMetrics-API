namespace BodyMetricsApi.Features.Athletes.Shared.ViewModels;

public sealed record ProfilePhotoViewModel(
    string BlobPath,
    string FileName,
    string ContentType,
    DateTimeOffset UploadedAtUtc,
    string? AccessUrl);

