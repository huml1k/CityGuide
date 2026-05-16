using ContentService.Application.Features.Tags.Queries.GetTags;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Api.Controllers
{
    [ApiController]
    [Route("api/tags")]
    public class TagsController : Controller
    {
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //get all
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetTagsQuery();

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
