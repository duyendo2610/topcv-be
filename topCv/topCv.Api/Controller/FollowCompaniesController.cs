using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using topCv.Application.Interfaces.Obj;
using topCv.Domain.Entities.Auth;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/follow-companies")]
    public class FollowCompaniesController : ControllerBase
    {
        private readonly IFollowCompanyService _service;

        public FollowCompaniesController(IFollowCompanyService service)
        {
            _service = service;
        }

        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [Authorize]
        [HttpPost("{companyId:guid}")]
        public async Task<IActionResult> Follow(Guid companyId, CancellationToken ct)
        {
            await _service.FollowAsync(companyId, UserId, ct);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{companyId:guid}")]
        public async Task<IActionResult> Unfollow(Guid companyId, CancellationToken ct)
        {
            await _service.UnfollowAsync(companyId, UserId, ct);
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMine(CancellationToken ct)
            => Ok(await _service.GetMyFollowedAsync(UserId, ct));
    }
}
