namespace BodyMetricsApi.Infrastructure.Storage;

public interface IAthletePhotoStorage
{
    Task<StoredAthletePhoto?> UploadAsync(AthletePhotoUpload? upload, string athleteId, CancellationToken cancellationToken);

    Task<Uri?> GetReadUrlAsync(string? blobPath, CancellationToken cancellationToken);
}

public sealed record AthletePhotoUpload(string FileName, string ContentType, byte[] Content);

public sealed record StoredAthletePhoto(string BlobPath, string FileName, string ContentType, DateTimeOffset UploadedAtUtc);

