using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/employer")]
    [Authorize]
    public class EmployerDashboardController : ControllerBase
    {
        private readonly IEmployerDashboardService _service;

        public EmployerDashboardController(IEmployerDashboardService service)
        {
            _service = service;
        }

        private Guid UserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Company của tôi
        [HttpGet("companies")]
        public async Task<IActionResult> GetMyCompanies(CancellationToken ct)
            => Ok(await _service.GetMyCompaniesAsync(UserId, ct));

        // Job theo company
        [HttpGet("companies/{companyId:guid}/jobs")]
        public async Task<IActionResult> GetJobs(Guid companyId, CancellationToken ct)
            => Ok(await _service.GetJobsByCompanyAsync(companyId, UserId, ct));
    }
}