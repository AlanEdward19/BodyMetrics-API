using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Sports;

public sealed record UpdateSportCommand(
    string Id,
    string Name,
    IReadOnlyList<string> Sectors,
    IReadOnlyList<string> Categories) : ICommand<SportResponse>;

