using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface IEmployerDashboardService
    {
        Task<List<EmployerCompanyResponse>> GetMyCompaniesAsync(Guid userId, CancellationToken ct);
        Task<List<EmployerJobResponse>> GetJobsByCompanyAsync(Guid companyId, Guid userId, CancellationToken ct);
    }
}