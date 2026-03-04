using Microsoft.AspNetCore.Mvc;
using topCv.Application.Interfaces.Commons;

namespace topCv.Api.Controller
{
    [ApiController]
    [Route("api/resume-templates")]
    public class ResumeTemplatesController : ControllerBase
    {
        private readonly IResumeTemplateCatalogService _service;

        public ResumeTemplatesController(IResumeTemplateCatalogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCatalog(CancellationToken ct)
            => Ok(await _service.GetCatalogAsync(ct));
    }
}
