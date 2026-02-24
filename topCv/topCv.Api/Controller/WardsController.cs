using Microsoft.AspNetCore.Mvc;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/wards")]
    public class WardsController : ControllerBase
    {
        private readonly IWardService _wardService;

        public WardsController(IWardService wardService)
        {
            _wardService = wardService;
        }

        // GET: api/wards
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _wardService.GetAllAsync();
            return Ok(result);
        }

        // GET: api/wards/by-province/{provinceId}
        [HttpGet("by-province/{provinceId:int}")]
        public async Task<IActionResult> GetByProvince(int provinceId)
        {
            var result = await _wardService.GetByProvinceIdAsync(provinceId);
            return Ok(result);
        }

        // GET: api/wards/search?keyword=ba dinh
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string keyword,
            CancellationToken ct)
        {
            var result = await _wardService.SearchAsync(keyword, ct);
            return Ok(result);
        }
    }
}