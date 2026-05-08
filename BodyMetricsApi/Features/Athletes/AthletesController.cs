using BodyMetricsApi.Features.Athletes.Create;
using BodyMetricsApi.Features.Athletes.Delete;
using BodyMetricsApi.Features.Athletes.GetAll;
using BodyMetricsApi.Features.Athletes.GetById;
using BodyMetricsApi.Features.Athletes.Shared.Enums;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Features.Athletes.Update;
using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BodyMetricsApi.Features.Athletes;

[ApiController]
[Authorize]
[Tags("Athletes")]
[Produces("application/json")]
[Route("api/athletes")]
public sealed class AthletesController : ControllerBase
{
    private const string GetAthleteByIdRouteName = "GetAthleteById";

    [HttpPost]
    [Consumes("application/json")]
    [EndpointSummary("Creates an athlete.")]
    [EndpointDescription("Creates a new athlete for the authenticated user, validates the selected sport options, and optionally uploads a profile photo.")]
    [ProducesResponseType(typeof(AthleteViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAthleteCommand command,
        [FromServices] CreateAthleteCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return this.ToActionResult(result);
        }

        return CreatedAtRoute(GetAthleteByIdRouteName, new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet]
    [EndpointSummary("Lists athletes.")]
    [EndpointDescription("Returns a paginated list of athletes that belong to the authenticated user, with optional filters for name, sport, sector, category, and phase.")]
    [ProducesResponseType(typeof(PagedResponseViewModel<AthleteViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? fullName,
        [FromQuery] string? sportId,
        [FromQuery] string? sector,
        [FromQuery] string? category,
        [FromQuery] Phase? phase,
        [FromServices] GetAllAthletesQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(
            new GetAllAthletesQuery(page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, fullName, sportId, sector, category, phase),
            cancellationToken);

        return this.ToActionResult(response);
    }

    [HttpGet("{id}", Name = GetAthleteByIdRouteName)]
    [EndpointSummary("Gets an athlete by id.")]
    [EndpointDescription("Returns the athlete owned by the authenticated user that matches the provided id.")]
    [ProducesResponseType(typeof(AthleteViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(
        string id,
        [FromServices] GetAthleteByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetAthleteByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("{id}")]
    [Consumes("application/json")]
    [EndpointSummary("Updates an athlete.")]
    [EndpointDescription("Updates the athlete owned by the authenticated user, replacing profile data, assessments, and optionally the profile photo.")]
    [ProducesResponseType(typeof(AthleteViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateAthleteCommand request,
        [FromServices] UpdateAthleteCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request with { Id = id };
        var result = await handler.HandleAsync(command, cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Deletes an athlete.")]
    [EndpointDescription("Deletes the athlete owned by the authenticated user that matches the provided id.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(
        string id,
        [FromServices] DeleteAthleteCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteAthleteCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
