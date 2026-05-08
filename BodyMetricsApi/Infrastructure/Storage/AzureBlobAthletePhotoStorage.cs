using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using BodyMetricsApi.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace BodyMetricsApi.Infrastructure.Storage;

public sealed class AzureBlobAthletePhotoStorage : IAthletePhotoStorage
{
    private readonly BlobContainerClient _containerClient;
    private readonly AthletePhotoStorageOptions _options;

    public AzureBlobAthletePhotoStorage(IOptions<AthletePhotoStorageOptions> options)
    {
        _options = options.Value;
        var serviceClient = new BlobServiceClient(_options.ConnectionString);
        _containerClient = serviceClient.GetBlobContainerClient(_options.ContainerName);
    }

    public async Task<StoredAthletePhoto?> UploadAsync(AthletePhotoUpload? upload, string athleteId, CancellationToken cancellationToken)
    {
        if (upload is null)
        {
            return null;
        }

        await _containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

        var sanitizedFileName = Path.GetFileName(upload.FileName).Replace(' ', '-');
        var blobPath = $"athletes/{athleteId}/{Guid.NewGuid():N}-{sanitizedFileName}";
        var blobClient = _containerClient.GetBlobClient(blobPath);

        await using var stream = new MemoryStream(upload.Content);
        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = upload.ContentType
            }
        }, cancellationToken);

        return new StoredAthletePhoto(blobPath, sanitizedFileName, upload.ContentType, DateTimeOffset.UtcNow);
    }

    public Task<Uri?> GetReadUrlAsync(string? blobPath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(blobPath))
        {
            return Task.FromResult<Uri?>(null);
        }

        var blobClient = _containerClient.GetBlobClient(blobPath);

        if (blobClient.CanGenerateSasUri)
        {
            var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(_options.ReadUrlExpirationMinutes));
            return Task.FromResult<Uri?>(sasUri);
        }

        return Task.FromResult<Uri?>(blobClient.Uri);
    }
}

