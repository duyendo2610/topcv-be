using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/employer")]
    [Authorize(Roles = "Employer")]
    public class EmployerController : ControllerBase
    {
        [HttpGet("post")]
        public IActionResult Dashboard()
            => Ok(new { message = "Welcome Employer" });
    }
}
