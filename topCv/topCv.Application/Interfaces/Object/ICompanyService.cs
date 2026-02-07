using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;

namespace topCv.Application.Interfaces.Obj
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
