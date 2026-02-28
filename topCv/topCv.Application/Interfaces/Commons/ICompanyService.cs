using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface ICompanyService
    {
        Task<CompanyResponse> CreateAsync(CreateCompanyRequest req, Guid userId, CancellationToken ct);
        Task<CompanyResponse> UpdateAsync(Guid id, UpdateCompanyRequest req, Guid userId, CancellationToken ct);
        Task<CompanyResponse> GetByIdAsync(Guid id, CancellationToken ct);
        Task<PagedResult<CompanyResponse>> GetAllAsync(CompanyQueryRequest req, CancellationToken ct);
        Task<PagedResult<CompanyResponse>> GetMyCompaniesAsync(Guid userId, CompanyQueryRequest req, CancellationToken ct);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken ct);
    }
}
