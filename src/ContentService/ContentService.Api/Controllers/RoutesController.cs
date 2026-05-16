using ContentService.Application.Features.Routes.Commands.CreateRoute;
using ContentService.Application.Features.Routes.Commands.DeleteRoute;
using ContentService.Application.Features.Routes.Commands.UpdateRoute;
using ContentService.Application.Features.Routes.Queries.GetRouteById;
using ContentService.Application.Features.Routes.Queries.GetRoutes;
using ContentService.Application.Features.Routes.Queries.SearchRoutes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ContentService.Api.Controllers
{
    [ApiController]
    [Route("api/routes")]
    public class RoutesController : Controller
    {
        private IMediator _mediator;

        public RoutesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //create

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRouteCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        //update
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRouteCommand command, CancellationToken cancellationToken)
        {
            command.RouteId = id;

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        //delete
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteRouteCommand { RouteId = id };

            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        //get by id
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById (Guid id, CancellationToken cancellationToken)
        {
            var query = new GetRouteByIdQuery { RouteId = id };

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        //get all
        [HttpGet]
        public async Task<IActionResult> GetAll (CancellationToken cancellationToken)
        {
            var query = new GetRoutesQuery();

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        //search
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string search,CancellationToken cancellationToken)
        {
            var query = new SearchRoutesQuery
            {
                Search = search
            };

            var result = await _mediator.Send(
                query,
                cancellationToken);

            return Ok(result);
        }
    }
}
