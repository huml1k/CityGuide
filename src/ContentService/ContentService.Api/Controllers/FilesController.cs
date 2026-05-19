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
        [ProducesResponseType(201)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
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
        [ProducesResponseType(201)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
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
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
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
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
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
        /// <returns>JSON ответ с данными файла и ссылкой для скачивания</returns>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Produces("application/json")]
        [HttpGet("images/{id:guid}")]
        public async Task<IActionResult> GetImage(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetRouteImageQuery { ImageId = id };

            var result = await _mediator.Send(query, cancellationToken);

            /*return Ok(new
            {
                url = result.Url,
                expiresAt = result.ExpiresAt,
                fileName = result.FileName,
                contentType = result.ContentType
            });*/
            return Redirect(result.Url);
        }

        /// <summary>
        /// Запрос на получение аудио
        /// </summary>
        /// <param name="id">Id аудио</param>
        /// <param name="cancellationToken"></param>
        /// <returns>JSON ответ с данными файла и ссылкой для скачивания</returns>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Produces("application/json")]
        [HttpGet("audio/{id:guid}")]
        public async Task<IActionResult> GetAudio(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetAudioFileQuery { AudioFileId = id };

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new
            {
                url = result.Url,
                expiresAt = result.ExpiresAt,
                fileName = result.FileName,
                contentType = result.ContentType
            });
        }
    }
}
