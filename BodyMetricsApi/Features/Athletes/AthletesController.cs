using BodyMetricsApi.Features.Athletes.Create;
using BodyMetricsApi.Features.Athletes.Delete;
using BodyMetricsApi.Features.Athletes.GetAll;
using BodyMetricsApi.Features.Athletes.GetById;
using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Features.Athletes.Update;
using BodyMetricsApi.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace BodyMetricsApi.Features.Athletes;

[ApiController]
[Route("api/athletes")]
public sealed class AthletesController : ControllerBase
{
    private const string GetAthleteByIdRouteName = "GetAthleteById";

    [HttpPost]
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
    public async Task<ActionResult<IReadOnlyList<AthleteViewModel>>> GetAll(
        [FromServices] GetAllAthletesQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new GetAllAthletesQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id}", Name = GetAthleteByIdRouteName)]
    public async Task<IActionResult> GetById(
        string id,
        [FromServices] GetAthleteByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetAthleteByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("{id}")]
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
    public async Task<IActionResult> Delete(
        string id,
        [FromServices] DeleteAthleteCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteAthleteCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}
