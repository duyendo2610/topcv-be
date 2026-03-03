using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface IResumeTemplateCatalogService
    {
        Task<ResumeTemplateCatalogResponse> GetCatalogAsync(CancellationToken ct);
    }
}
