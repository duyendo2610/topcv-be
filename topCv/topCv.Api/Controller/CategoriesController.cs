using Microsoft.AspNetCore.Mvc;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(await _service.GetAllAsync(ct));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest req, CancellationToken ct)
            => Ok(await _service.CreateAsync(req, ct));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest req, CancellationToken ct)
            => Ok(await _service.UpdateAsync(id, req, ct));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }

        [HttpGet("tree")]
        public async Task<IActionResult> GetTree(CancellationToken ct)
        {
            var res = await _service.GetTreeAsync(ct);
            return Ok(res);
        }
    }
}