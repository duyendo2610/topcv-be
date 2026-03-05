using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/employer-requests")]
    [Authorize]
    public class EmployerRequestsController : ControllerBase
    {
        private readonly IEmployerRequestService _service;

        public EmployerRequestsController(IEmployerRequestService service)
        {
            _service = service;
        }

        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployerRequestRequest req, CancellationToken ct)
            => Ok(await _service.CreateAsync(req, UserId, ct));

        [HttpGet("me")]
        public async Task<IActionResult> GetMine(CancellationToken ct)
            => Ok(await _service.GetMineAsync(UserId, ct));
    }
}
