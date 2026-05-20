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
        public async Task<IActionResult> GetUnreadAsync(CancellationToken ct)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Ok(Array.Empty<object>());
            }

            var notifications = await _repository.GetUnreadAsync(userId, ct);
            return Ok(notifications);
        }

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

        private bool TryGetCurrentUserId(out Guid userId)
        {
            userId = default;
            var claim =
                User.FindFirst("sub")
                ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            return claim is not null && Guid.TryParse(claim.Value, out userId);
        }

        private Guid GetCurrentUserId()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }

            return userId;
        }
    }
}
