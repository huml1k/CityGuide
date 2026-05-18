using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Infrastructure.Repository.Interface;
using System.Security.Claims;

namespace NotificationService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : Controller
    {
        private readonly INotificationRepository _repository;

        public NotificationsController(INotificationRepository repository) 
        {
            _repository = repository;
        }

        [HttpGet("healthCheck")]
        public async Task<IActionResult> GetHealthCheck()
        {
            return Ok("service = NotifactionsService\n" +
                "status = Healthy\n" +
                $"timestamp = {DateTime.UtcNow}");
        }


        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadAsync(CancellationToken ct) =>
             Ok(await _repository.GetUnreadAsync(GetCurrentUserId(), ct));

        [HttpPost("{id:guid}/read")]
        public async Task<IActionResult> MarkAsReadAsync(Guid id, CancellationToken ct)
        {
            var notification = await _repository.GetByIdAsync(id, ct);
            if (notification == null || notification.UserId != GetCurrentUserId())
                return NotFound();

            notification.IsRead = true;
            await _repository.UpdateAsync(notification, ct);
            await _repository.SaveChangesAsync(ct);
            return NoContent();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsReadAsync(CancellationToken ct)
        {
            await _repository.MarkAllAsReadAsync(GetCurrentUserId(), ct);
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || !Guid.TryParse(claim.Value, out var userId))
                throw new UnauthorizedAccessException("User ID not found in token.");
            return userId;
        }
    }
}
