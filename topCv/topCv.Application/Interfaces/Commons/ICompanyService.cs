using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface ICompanyService
    {
        Task<CompanyResponse> CreateAsync(CreateCompanyRequest req, Guid userId, CancellationToken ct);
        Task<CompanyResponse> UpdateAsync(Guid id, UpdateCompanyRequest req, Guid userId, CancellationToken ct);
        Task<CompanyResponse> GetByIdAsync(Guid id, CancellationToken ct);
        Task<List<CompanyResponse>> GetAllAsync(CancellationToken ct);
        Task<List<CompanyResponse>> GetMyCompaniesAsync(Guid userId, CancellationToken ct);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken ct);
    }
}