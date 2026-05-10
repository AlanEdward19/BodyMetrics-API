using System.Collections.Concurrent;

namespace BodyMetricsApi.Infrastructure.Storage;

public sealed class InMemoryAthletePhotoStorage : IAthletePhotoStorage
{
    private readonly ConcurrentDictionary<string, AthletePhotoUpload> _photos = new();

    public Task<StoredAthletePhoto?> UploadAsync(AthletePhotoUpload? upload, string athleteId, CancellationToken cancellationToken)
    {
        if (upload is null)
        {
            return Task.FromResult<StoredAthletePhoto?>(null);
        }

        var blobPath = $"athletes/{athleteId}/{Guid.NewGuid():N}-{Path.GetFileName(upload.FileName)}";
        _photos[blobPath] = upload;

        return Task.FromResult<StoredAthletePhoto?>(
            new StoredAthletePhoto(blobPath, Path.GetFileName(upload.FileName), upload.ContentType, DateTimeOffset.UtcNow));
    }

    public Task<Uri?> GetReadUrlAsync(string? blobPath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(blobPath) || !_photos.ContainsKey(blobPath))
        {
            return Task.FromResult<Uri?>(null);
        }

        return Task.FromResult<Uri?>(new Uri($"https://photos.local/{blobPath}"));
    }

    public Task DeleteAsync(string? blobPath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(blobPath))
        {
            return Task.CompletedTask;
        }

        _photos.TryRemove(blobPath, out _);
        return Task.CompletedTask;
    }
}

