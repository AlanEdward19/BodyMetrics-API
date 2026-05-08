namespace BodyMetricsApi.Shared.ViewModels;

public sealed record PagedResponseViewModel<TItem>(
    IReadOnlyList<TItem> Items,
    int Page,
    int PageSize,
    long TotalCount,
    int TotalPages);

