using BodyMetricsApi.Features.Sports.Create;
using BodyMetricsApi.Features.Sports.Delete;
using BodyMetricsApi.Features.Sports.GetAll;
using BodyMetricsApi.Features.Sports.GetById;
using BodyMetricsApi.Features.Sports.Update;
using BodyMetricsApi.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace BodyMetricsApi.Features.Sports;

[ApiController]
[Route("api/sports")]
public sealed class SportsController : ControllerBase
{
    private const string GetSportByIdRouteName = "GetSportById";

    [HttpPost]
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
    public async Task<ActionResult<IReadOnlyList<SportResponse>>> GetAll(
        [FromServices] GetAllSportsQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new GetAllSportsQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id}", Name = GetSportByIdRouteName)]
    public async Task<IActionResult> GetById(
        string id,
        [FromServices] GetSportByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetSportByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("{id}")]
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
    public async Task<IActionResult> Delete(
        string id,
        [FromServices] DeleteSportCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteSportCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }
}



