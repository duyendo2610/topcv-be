using Microsoft.AspNetCore.Mvc;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/provinces")]
    public class ProvinceController : ControllerBase
    {
        private readonly IProvinceService _service;

        public ProvinceController(IProvinceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(await _service.GetAllAsync(ct));

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword, CancellationToken ct)
        {
            var result = await _service.SearchAsync(keyword, ct);
            return Ok(result);
        }
    }
}