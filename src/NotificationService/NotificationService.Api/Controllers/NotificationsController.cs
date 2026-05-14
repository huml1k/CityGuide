using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : Controller
    {
        [HttpGet("healthCheck")]
        public async Task<IActionResult> GetHealthCheck()
        {
            return Ok("service = NotifactionsService\n" +
                "status = Healthy\n" +
                $"timestamp = {DateTime.UtcNow}");
        }
    }
}
