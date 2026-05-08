namespace BodyMetricsApi.Infrastructure.Storage;

public sealed record AthletePhotoUpload(string FileName, string ContentType, byte[] Content);

