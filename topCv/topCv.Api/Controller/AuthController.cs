using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using topCv.Application.DTOs.Auth;
using topCv.Application.Interfaces.Auth;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest req, CancellationToken ct)
        {
            var res = await _auth.RegisterAsync(req, ct);
            return Ok(res);
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
        public IActionResult Me()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirstValue(ClaimTypes.Email);
            return Ok(new { userId, email });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest req,CancellationToken ct)
        {
            await _auth.LogoutAsync(req.RefreshToken, ct);
            return Ok(new { message = "Logout success" });
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll(CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _auth.LogoutAllAsync(userId, ct);

            return Ok(new { message = "Logout all devices success" });
        }
    }
}
