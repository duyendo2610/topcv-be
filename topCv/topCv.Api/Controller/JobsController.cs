using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/jobs")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _service;

        public JobsController(IJobService service)
        {
            _service = service;
        }

        private Guid UserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJobRequest req, CancellationToken ct)
            => Ok(await _service.CreateAsync(req, UserId, ct));

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobRequest req, CancellationToken ct)
            => Ok(await _service.UpdateAsync(id, req, UserId, ct));

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, UserId, ct);
            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => Ok(await _service.GetByIdAsync(id, ct));

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] JobQueryRequest req, CancellationToken ct)
            => Ok(await _service.SearchAsync(req, ct));

        [Authorize]
        [HttpPatch("{id:guid}/publish")]
        public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
        {
            await _service.PublishAsync(id, UserId, ct);
            return NoContent();
        }

        [Authorize]
        [HttpPatch("{id:guid}/close")]
        public async Task<IActionResult> Close(Guid id, CancellationToken ct)
        {
            await _service.CloseAsync(id, UserId, ct);
            return NoContent();
        }
    }
}