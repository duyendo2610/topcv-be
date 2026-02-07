using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using topCv.Application.Interfaces.Obj;
using topCv.Domain.Entities.Auth;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/job-matches")]
    [Authorize]
    public class JobMatchesController : ControllerBase
    {
        private readonly IJobMatchService _service;

        public JobMatchesController(IJobMatchService service)
        {
            _service = service;
        }

        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("me")]
        public async Task<IActionResult> GetMine([FromQuery] int take = 20, CancellationToken ct = default)
            => Ok(await _service.GetMyMatchesAsync(UserId, take, ct));

        [HttpPost("me/notify")]
        public async Task<IActionResult> NotifyMine([FromQuery] int take = 10, CancellationToken ct = default)
        {
            var created = await _service.NotifyMyMatchesAsync(UserId, take, ct);
            return Ok(new { created });
        }
    }
}
