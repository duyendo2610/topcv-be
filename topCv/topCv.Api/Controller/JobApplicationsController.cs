using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/applications")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IJobApplicationService _service;

        public ApplicationsController(IJobApplicationService service)
        {
            _service = service;
        }

        private Guid UserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Candidate: Apply job
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Apply([FromBody] CreateJobApplicationRequest req, CancellationToken ct)
            => Ok(await _service.ApplyAsync(req, UserId, ct));

        // Candidate: list my applications
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMine(CancellationToken ct)
            => Ok(await _service.GetMyApplicationsAsync(UserId, ct));

        // Employer: list applicants by job
        [Authorize]
        [HttpGet("by-job/{jobId:guid}")]
        public async Task<IActionResult> GetByJob(Guid jobId, CancellationToken ct)
            => Ok(await _service.GetByJobAsync(jobId, UserId, ct));

        // Employer: update application status
        [Authorize]
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateJobApplicationStatusRequest req,
            CancellationToken ct)
            => Ok(await _service.UpdateStatusAsync(id, req, UserId, ct));

        // Candidate: update my application content (optional endpoint)
        [Authorize]
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateMy(Guid id, [FromBody] UpdateJobApplicationRequest req,
            CancellationToken ct)
            => Ok(await _service.UpdateMyApplicationAsync(id, req, UserId, ct));
    }
}