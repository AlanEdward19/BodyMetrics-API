namespace BodyMetricsApi.Features.Sports.Shared.Interfaces;

/// <summary>
/// Contract for Sport write operations (Create/Update).
/// Used by the base SportWriteCommandValidator to enforce common validation rules.
/// </summary>
public interface ISportWriteCommand
{
    string Name { get; }
    IReadOnlyList<string> Sectors { get; }
    IReadOnlyList<string> Categories { get; }
}

