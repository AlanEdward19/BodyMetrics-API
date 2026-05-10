namespace BodyMetricsApi.Features.Sports.Shared.ViewModels;

public sealed record SportResponse(string Id, string Name, IReadOnlyList<string> Sectors, IReadOnlyList<string> Categories);

