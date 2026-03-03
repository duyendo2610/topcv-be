using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
            => Ok(new { message = "Chào mừng quản trị viên" });
    }
}
