using BodyMetricsApi.Features.Sports.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;
using BodyMetricsApi.Shared.ViewModels;

namespace BodyMetricsApi.Features.Sports.GetAll;

public sealed record GetAllSportsQuery(
	int Page,
	int PageSize,
	string? Name,
	string? Sector,
	string? Category) : IQuery<PagedResponseViewModel<SportResponse>>;

