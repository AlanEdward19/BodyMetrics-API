namespace BodyMetricsApi.Infrastructure.Configuration;

public sealed class FirebaseAuthenticationOptions
{
    public const string SectionName = "FirebaseAuthentication";

    public string ProjectId { get; init; } = "configure-me";

    public string Issuer => $"https://securetoken.google.com/{ProjectId}";
}

