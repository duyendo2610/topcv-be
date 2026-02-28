using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _service;

        public CompaniesController(ICompanyService service)
        {
            _service = service;
        }

        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCompanyRequest req, CancellationToken ct)
        {
            return Ok(await _service.CreateAsync(req, UserId, ct));
        }


        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCompanyRequest req, CancellationToken ct)
            => Ok(await _service.UpdateAsync(id, req, UserId, ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => Ok(await _service.GetByIdAsync(id, ct));

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CompanyQueryRequest req, CancellationToken ct)
            => Ok(await _service.GetAllAsync(req, ct));

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMine([FromQuery] CompanyQueryRequest req, CancellationToken ct)
            => Ok(await _service.GetMyCompaniesAsync(UserId, req, ct));

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, UserId, ct);
            return NoContent();
        }
    }
}
