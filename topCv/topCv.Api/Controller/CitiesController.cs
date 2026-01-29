using Microsoft.AspNetCore.Mvc;
using topCv.Application.DTOs.Obj;
using topCv.Application.Interfaces.Obj;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _service;

        public CitiesController(ICityService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(await _service.GetAllAsync(ct));

        [HttpPost]
        public async Task<IActionResult> Create(CreateNameRequest req, CancellationToken ct)
            => Ok(await _service.CreateAsync(req, ct));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateNameRequest req, CancellationToken ct)
            => Ok(await _service.UpdateAsync(id, req, ct));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
    }

}
