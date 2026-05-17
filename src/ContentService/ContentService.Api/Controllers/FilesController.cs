using ContentService.Application.Features.Files.Commands.DeleteAudioFile;
using ContentService.Application.Features.Files.Commands.DeleteRouteImage;
using ContentService.Application.Features.Files.Commands.UploadAudioFile;
using ContentService.Application.Features.Files.Commands.UploadRouteImage;
using ContentService.Application.Features.Files.Queries.GetAudioFile;
using ContentService.Application.Features.Files.Queries.GetRouteImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Api.Controllers
{
    [ApiController]
    [Route("api/files")]
    [Authorize]
    public class FilesController : Controller
    {
        private readonly IMediator _mediator;

        public FilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // upload image

        [HttpPost("images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] UploadRouteImageCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        // upload audio

        [HttpPost("audio")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAudio([FromForm] UploadAudioFileCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        // delete image

        [HttpDelete("images/{id:guid}")]
        public async Task<IActionResult> DeleteImage(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteRouteImageCommand { ImageId = id };

            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        // delete audio

        [HttpDelete("audio/{id:guid}")]
        public async Task<IActionResult> DeleteAudio(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteAudioFileCommand { AudioFileId = id };

            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        // get image

        [AllowAnonymous]
        [HttpGet("images/{id:guid}")]
        public async Task<IActionResult> GetImage(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetRouteImageQuery { ImageId = id };

            var result = await _mediator.Send(query, cancellationToken);

            return File(result.Stream, result.ContentType, result.FileName);
        }

        // get audio

        [AllowAnonymous]
        [HttpGet("audio/{id:guid}")]
        public async Task<IActionResult> GetAudio(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetAudioFileQuery { AudioFileId = id };

            var result = await _mediator.Send(query, cancellationToken);

            return File(result.Stream, result.ContentType, result.FileName);
        }
    }
}
