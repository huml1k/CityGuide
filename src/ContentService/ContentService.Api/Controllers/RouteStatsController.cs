using ContentService.Application.Features.RouteStats.Commands.DecrementFavoritesCount;
using ContentService.Application.Features.RouteStats.Commands.IncrementFavoritesCount;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Api.Controllers;

[ApiController]
[Route("api/route-stats")]
public class RouteStatsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RouteStatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{routeId:guid}/favorite/increment")]
    public async Task<IActionResult> IncrementFavorites(
        Guid routeId,
        CancellationToken cancellationToken)
    {
        var command = new IncrementFavoritesCountCommand
        {
            RouteId = routeId
        };

        await _mediator.Send(
            command,
            cancellationToken);

        return Ok();
    }

    [HttpPost("{routeId:guid}/favorite/decrement")]
    public async Task<IActionResult> DecrementFavorites(
        Guid routeId,
        CancellationToken cancellationToken)
    {
        var command = new DecrementFavoritesCountCommand
        {
            RouteId = routeId
        };

        await _mediator.Send(
            command,
            cancellationToken);

        return Ok();
    }
}