namespace BodyMetricsApi.Features.Sports;

public sealed record SportResponse(string Id, string Name, IReadOnlyList<string> Sectors, IReadOnlyList<string> Categories);

