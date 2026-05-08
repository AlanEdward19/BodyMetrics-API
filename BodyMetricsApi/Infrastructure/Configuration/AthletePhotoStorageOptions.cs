namespace BodyMetricsApi.Infrastructure.Configuration;

public sealed class AthletePhotoStorageOptions
{
    public const string SectionName = "AthletePhotoStorage";

    public string Provider { get; init; } = "AzureBlob";

    public string ConnectionString { get; init; } = "UseDevelopmentStorage=true";

    public string ContainerName { get; init; } = "athlete-photos";

    public int ReadUrlExpirationMinutes { get; init; } = 60;
}

