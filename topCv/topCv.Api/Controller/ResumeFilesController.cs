using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/resume-files")]
    public class ResumeFilesController : ControllerBase
    {
        private readonly IResumeFileService _service;

        public ResumeFilesController(IResumeFileService service)
        {
            _service = service;
        }

        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [Authorize]
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadResumeFileForm form, CancellationToken ct)
        {
            if (form.File is null) return BadRequest("File is required.");

            await using var stream = form.File.OpenReadStream();

            var res = await _service.UploadAsync(
                UserId,
                form.ResumeId,
                stream,
                form.File.FileName,
                form.File.ContentType,
                form.File.Length,
                ct);

            return Ok(res);
        }

        [Authorize]
        [HttpGet("by-resume/{resumeId:guid}")]
        public async Task<IActionResult> GetByResume(Guid resumeId, CancellationToken ct)
            => Ok(await _service.GetByResumeAsync(UserId, resumeId, ct));

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _service.DeleteAsync(UserId, id, ct);
            return NoContent();
        }
    }
}