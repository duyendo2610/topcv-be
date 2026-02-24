using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("me")]
        public async Task<IActionResult> GetMine(CancellationToken ct)
            => Ok(await _service.GetMyAsync(UserId, ct));

        [HttpPatch("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct)
        {
            await _service.MarkAsReadAsync(id, UserId, ct);
            return NoContent();
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
        {
            await _service.MarkAllAsReadAsync(UserId, ct);
            return NoContent();
        }
    }
}