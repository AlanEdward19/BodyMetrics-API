namespace BodyMetricsApi.Infrastructure.Storage;

public sealed record StoredAthletePhoto(string BlobPath, string FileName, string ContentType, DateTimeOffset UploadedAtUtc);

