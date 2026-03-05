using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/admin/employer-requests")]
    [Authorize(Roles = "Admin")]
    public class AdminEmployerRequestsController : ControllerBase
    {
        private readonly IEmployerRequestService _service;

        public AdminEmployerRequestsController(IEmployerRequestService service)
        {
            _service = service;
        }

        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetPending(CancellationToken ct)
            => Ok(await _service.GetPendingAsync(ct));

        [HttpPost("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id, CancellationToken ct)
        {
            await _service.ApproveAsync(id, UserId, ct);
            return NoContent();
        }

        [HttpPost("{id:guid}/reject")]
        public async Task<IActionResult> Reject(Guid id, CancellationToken ct)
        {
            await _service.RejectAsync(id, UserId, ct);
            return NoContent();
        }
    }
}
