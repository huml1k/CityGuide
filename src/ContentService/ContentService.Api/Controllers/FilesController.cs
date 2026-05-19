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
        
        /// <summary>
        /// Отправляет файл изображения для указанного пути
        /// </summary>
        /// <param name="command">Запрос на создание изображения</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Результат</returns>
        [HttpPost("images")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] UploadRouteImageCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
        
        /// <summary>
        /// Отправляет файл аудио для указанного пути
        /// </summary>
        /// <param name="command">Запрос на создание аудио</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Результат</returns>
        [HttpPost("audio")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAudio([FromForm] UploadAudioFileCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Удаляет изображение из базы
        /// </summary>
        /// <param name="id">Id изображения</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Результат</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("images/{id:guid}")]
        public async Task<IActionResult> DeleteImage(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteRouteImageCommand { ImageId = id };

            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Удаляет аудио из базы
        /// </summary>
        /// <param name="id">Id аудио</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Результат</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("audio/{id:guid}")]
        public async Task<IActionResult> DeleteAudio(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteAudioFileCommand { AudioFileId = id };

            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Запрос на получение изображения
        /// </summary>
        /// <param name="id">Id изображения</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Файл</returns>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("images/{id:guid}")]
        public async Task<IActionResult> GetImage(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetRouteImageQuery { ImageId = id };

            var result = await _mediator.Send(query, cancellationToken);

            return File(result.Stream, result.ContentType, result.FileName);
        }

        /// <summary>
        /// Запрос на получение аудио
        /// </summary>
        /// <param name="id">Id аудио</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Файл</returns>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("audio/{id:guid}")]
        public async Task<IActionResult> GetAudio(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetAudioFileQuery { AudioFileId = id };

            var result = await _mediator.Send(query, cancellationToken);

            return File(result.Stream, result.ContentType, result.FileName);
        }
    }
}
