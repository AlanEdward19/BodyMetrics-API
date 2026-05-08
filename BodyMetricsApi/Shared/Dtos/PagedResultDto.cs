namespace BodyMetricsApi.Shared.Dtos;

public sealed record PagedResultDto<TItem>(IReadOnlyList<TItem> Items, long TotalCount);

