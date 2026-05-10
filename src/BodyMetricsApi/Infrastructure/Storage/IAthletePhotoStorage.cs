namespace BodyMetricsApi.Infrastructure.Storage;

public interface IAthletePhotoStorage
{
    Task<StoredAthletePhoto?> UploadAsync(AthletePhotoUpload? upload, string athleteId, CancellationToken cancellationToken);

    Task<Uri?> GetReadUrlAsync(string? blobPath, CancellationToken cancellationToken);
}

