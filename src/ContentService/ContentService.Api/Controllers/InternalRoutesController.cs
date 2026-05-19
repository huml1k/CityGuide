using ContentService.Application.Features.Routes.Queries.GetPendingRoutes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ContentService.Application.Features.Routes.Commands.ApproveRoute;
using ContentService.Application.Features.Routes.Commands.RejectRoute;

namespace ContentService.Api.Controllers
{
    [ApiController]
    [Route("internal/routes")]
    //[Authorize(Roles = "Admin")]
    public class InternalRoutesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InternalRoutesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRoutes(
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetPendingRoutesQuery(),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("{routeId:guid}/approve")]
        public async Task<IActionResult> ApproveRoute(Guid routeId, CancellationToken cancellationToken)
        {
            await _mediator.Send( new ApproveRouteCommand(routeId), cancellationToken);

            return NoContent();
        }

        [HttpPost("{routeId:guid}/reject")]
        public async Task<IActionResult> RejectRoute(Guid routeId,CancellationToken cancellationToken)
        {
            await _mediator.Send(new RejectRouteCommand(routeId), cancellationToken);

            return NoContent();
        }
    }
}
