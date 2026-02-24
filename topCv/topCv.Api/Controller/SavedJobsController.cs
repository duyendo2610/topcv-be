using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/saved-jobs")]
    public class SavedJobsController : ControllerBase
    {
        private readonly ISavedJobService _service;

        public SavedJobsController(ISavedJobService service)
        {
            _service = service;
        }

        private Guid UserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [Authorize]
        [HttpPost("{jobId:guid}")]
        public async Task<IActionResult> Save(Guid jobId, CancellationToken ct)
        {
            await _service.SaveAsync(jobId, UserId, ct);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{jobId:guid}")]
        public async Task<IActionResult> Unsave(Guid jobId, CancellationToken ct)
        {
            await _service.UnsaveAsync(jobId, UserId, ct);
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMine(CancellationToken ct)
            => Ok(await _service.GetMySavedJobsAsync(UserId, ct));
    }
}