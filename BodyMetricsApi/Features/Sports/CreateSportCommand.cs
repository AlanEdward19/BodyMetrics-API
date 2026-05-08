using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Sports;

public sealed record CreateSportCommand(
    string Name,
    IReadOnlyList<string> Sectors,
    IReadOnlyList<string> Categories) : ICommand<SportResponse>;

