using BodyMetricsApi.Features.Sports.Shared.Interfaces;
using BodyMetricsApi.Features.Sports.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;

namespace BodyMetricsApi.Features.Sports.Create;

public sealed record CreateSportCommand(
    string Name,
    IReadOnlyList<string> Sectors,
    IReadOnlyList<string> Categories) : ICommand<SportResponse>, ISportWriteCommand;


