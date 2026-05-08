using BodyMetricsApi.Features.Athletes.Shared.Enums;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Shared.CQRS;
using BodyMetricsApi.Shared.ViewModels;

namespace BodyMetricsApi.Features.Athletes.GetAll;

public sealed record GetAllAthletesQuery(
	int Page,
	int PageSize,
	string? FullName,
	string? SportId,
	string? Sector,
	string? Category,
	Phase? Phase) : IQuery<PagedResponseViewModel<AthleteViewModel>>;
