using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Auth;
using topCv.Application.Interfaces.Auth;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IAppDbContext _db;

        public AuthController(IAuthService auth, IAppDbContext db)
        {
            _auth = auth;
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest req, CancellationToken ct)
        {
            var res = await _auth.RegisterAsync(req, ct);
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest req, CancellationToken ct)
        {
            var res = await _auth.LoginAsync(req, ct);
            return Ok(res);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] UserRefreshTokenRequest req, CancellationToken ct)
        {
            var res = await _auth.RefreshAsync(req, ct);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me(CancellationToken ct)
        {
            var userIdRaw =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!Guid.TryParse(userIdRaw, out var userId))
            {
                return Unauthorized(new { message = "Token missing or invalid user id claim." });
            }

            var me = await _db.Users
                .AsNoTracking()
                .Where(x => x.Id == userId)
                .Select(x => new
                {
                    x.Id,
                    x.Email,
                    x.FullName,
                    x.Phone,
                    x.Role,
                    x.IsActive,
                    x.CreatedAtUtc
                })
                .FirstOrDefaultAsync(ct);

            if (me is null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(me);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest req, CancellationToken ct)
        {
            await _auth.LogoutAsync(req.RefreshToken, ct);
            return Ok(new { message = "Logout success" });
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll(CancellationToken ct)
        {
            var userIdRaw =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!Guid.TryParse(userIdRaw, out var userId))
            {
                return Unauthorized(new { message = "Token missing or invalid user id claim." });
            }

            await _auth.LogoutAllAsync(userId, ct);

            return Ok(new { message = "Logout all devices success" });
        }
    }
}
