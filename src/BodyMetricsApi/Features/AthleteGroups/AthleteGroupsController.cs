using BodyMetricsApi.Features.Athletes.Shared.ViewModels;
using BodyMetricsApi.Features.AthleteGroups.AddMember;
using BodyMetricsApi.Features.AthleteGroups.Compare;
using BodyMetricsApi.Features.AthleteGroups.Create;
using BodyMetricsApi.Features.AthleteGroups.Delete;
using BodyMetricsApi.Features.AthleteGroups.GetAll;
using BodyMetricsApi.Features.AthleteGroups.GetById;
using BodyMetricsApi.Features.AthleteGroups.GetMembers;
using BodyMetricsApi.Features.AthleteGroups.RemoveMember;
using BodyMetricsApi.Features.AthleteGroups.Shared.ViewModels;
using BodyMetricsApi.Features.AthleteGroups.Update;
using BodyMetricsApi.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BodyMetricsApi.Features.AthleteGroups;

[ApiController]
[Authorize]
[Tags("AthleteGroups")]
[Produces("application/json")]
[Route("api/athlete-groups")]
public sealed class AthleteGroupsController : ControllerBase
{
    private const string GetAthleteGroupByIdRouteName = "GetAthleteGroupById";

    [HttpPost]
    [Consumes("application/json")]
    [EndpointSummary("Creates an athlete group.")]
    [EndpointDescription("Creates a new athlete group for the authenticated user with a unique name.")]
    [ProducesResponseType(typeof(AthleteGroupViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAthleteGroupCommand command,
        [FromServices] CreateAthleteGroupCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return this.ToActionResult(result);
        }

        return CreatedAtRoute(GetAthleteGroupByIdRouteName, new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet]
    [EndpointSummary("Lists athlete groups.")]
    [EndpointDescription("Returns all athlete groups belonging to the authenticated user, ordered by name.")]
    [ProducesResponseType(typeof(List<AthleteGroupViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromServices] GetAllAthleteGroupsQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetAllAthleteGroupsQuery(), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{id}", Name = GetAthleteGroupByIdRouteName)]
    [EndpointSummary("Gets an athlete group by id.")]
    [EndpointDescription("Returns the athlete group owned by the authenticated user that matches the provided id.")]
    [ProducesResponseType(typeof(AthleteGroupViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(
        string id,
        [FromServices] GetAthleteGroupByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetAthleteGroupByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("{id}")]
    [Consumes("application/json")]
    [EndpointSummary("Updates an athlete group.")]
    [EndpointDescription("Renames the athlete group owned by the authenticated user.")]
    [ProducesResponseType(typeof(AthleteGroupViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateAthleteGroupCommand request,
        [FromServices] UpdateAthleteGroupCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request with { Id = id };
        var result = await handler.HandleAsync(command, cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Deletes an athlete group.")]
    [EndpointDescription("Deletes the athlete group owned by the authenticated user. Associated athletes are not deleted.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(
        string id,
        [FromServices] DeleteAthleteGroupCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteAthleteGroupCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("{id}/members")]
    [EndpointSummary("Lists the athletes in a group.")]
    [EndpointDescription("Returns the athletes embedded in the group owned by the authenticated user.")]
    [ProducesResponseType(typeof(List<AthleteViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMembers(
        string id,
        [FromServices] GetAthleteGroupMembersQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetAthleteGroupMembersQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("{id}/members/{athleteId}")]
    [EndpointSummary("Adds an athlete to a group.")]
    [EndpointDescription("Adds the specified athlete (owned by the authenticated user) to the group. The athlete is physically moved into the group's embedded member list, from wherever it currently lives (standalone or another group) — this also acts as \"move athlete to group\". Idempotent — adding an existing member returns 204.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddMember(
        string id,
        string athleteId,
        [FromServices] AddAthleteToGroupCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new AddAthleteToGroupCommand(id, athleteId), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpDelete("{id}/members/{athleteId}")]
    [EndpointSummary("Removes an athlete from a group.")]
    [EndpointDescription("Removes the specified athlete from the group and returns it to the standalone (ungrouped) athlete list. Idempotent — removing a non-member returns 204. The athlete is not deleted from the system.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveMember(
        string id,
        string athleteId,
        [FromServices] RemoveAthleteFromGroupCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new RemoveAthleteFromGroupCommand(id, athleteId), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("comparison")]
    [EndpointSummary("Compares physical indicators across groups.")]
    [EndpointDescription("Returns aggregated physical metrics (average, min, max, median) per group based on each athlete's most recent physical assessment. Requires at least two group IDs.")]
    [ProducesResponseType(typeof(List<AthleteGroupComparisonViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Compare(
        [FromQuery] List<string> groupIds,
        [FromServices] CompareAthleteGroupsQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new CompareAthleteGroupsQuery(groupIds), cancellationToken);
        return this.ToActionResult(result);
    }
}
