using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/resumes")]
    public class ResumesController : ControllerBase
    {
        private readonly IResumeService _service;

        public ResumesController(IResumeService service)
        {
            _service = service;
        }

        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMine(CancellationToken ct)
            => Ok(await _service.GetMineAsync(UserId, ct));
        
        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => Ok(await _service.GetByIdAsync(UserId, id, ct));

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateResumeRequest req, CancellationToken ct)
            => Ok(await _service.CreateAsync(UserId, req, ct));

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateResumeRequest req, CancellationToken ct)
            => Ok(await _service.UpdateAsync(UserId, id, req, ct));

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _service.DeleteAsync(UserId, id, ct);
            return NoContent();
        }
        
        [Authorize]
        [HttpGet("{id:guid}/preview")]
        public async Task<IActionResult> Preview(Guid id, CancellationToken ct)
            => Ok(await _service.PreviewAsync(UserId, id, ct));
        
        [Authorize]
        [HttpPost("{id:guid}/exports/html")]
        public async Task<IActionResult> ExportHtml(Guid id, CancellationToken ct)
            => Ok(await _service.ExportHtmlAsync(UserId, id, ct));
    }
}
