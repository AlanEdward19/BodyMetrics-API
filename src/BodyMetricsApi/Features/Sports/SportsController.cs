using BodyMetricsApi.Features.Sports.Create;
using BodyMetricsApi.Features.Sports.Delete;
using BodyMetricsApi.Features.Sports.GetAll;
using BodyMetricsApi.Features.Sports.GetById;
using BodyMetricsApi.Features.Sports.Shared.ViewModels;
using BodyMetricsApi.Features.Sports.Update;
using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BodyMetricsApi.Features.Sports;

[ApiController]
[Authorize]
[Tags("Sports")]
[Produces("application/json")]
[Route("api/sports")]
public sealed class SportsController : ControllerBase
{
    private const string GetSportByIdRouteName = "GetSportById";

    [HttpPost]
    [Consumes("application/json")]
    [EndpointSummary("Creates a sport.")]
    [EndpointDescription("Creates a new sport with its valid sector and category options.")]
    [ProducesResponseType(typeof(SportResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSportCommand command,
        [FromServices] CreateSportCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return this.ToActionResult(result);
        }

        return CreatedAtRoute(GetSportByIdRouteName, new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet]
    [EndpointSummary("Lists sports.")]
    [EndpointDescription("Returns a paginated list of sports filtered by name, sector, and category.")]
    [ProducesResponseType(typeof(PagedResponseViewModel<SportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? name,
        [FromQuery] string? sector,
        [FromQuery] string? category,
        [FromServices] GetAllSportsQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(
            new GetAllSportsQuery(page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, name, sector, category),
            cancellationToken);

        return this.ToActionResult(response);
    }

    [HttpGet("{id}", Name = GetSportByIdRouteName)]
    [EndpointSummary("Gets a sport by id.")]
    [EndpointDescription("Returns the sport identified by the provided id.")]
    [ProducesResponseType(typeof(SportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(
        string id,
        [FromServices] GetSportByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetSportByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("{id}")]
    [Consumes("application/json")]
    [EndpointSummary("Updates a sport.")]
    [EndpointDescription("Replaces the sport name, sectors, and categories for the provided id.")]
    [ProducesResponseType(typeof(SportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateSportCommand request,
        [FromServices] UpdateSportCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request with { Id = id };
        var result = await handler.HandleAsync(command, cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Deletes a sport.")]
    [EndpointDescription("Removes the sport identified by the provided id.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(
        string id,
        [FromServices] DeleteSportCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteSportCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}



