using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;

namespace topCv.Application.Services.Obj
{
    public interface IEmployerDashboardService
    {
        Task<List<EmployerCompanyResponse>> GetMyCompaniesAsync(Guid userId, CancellationToken ct);
        Task<List<EmployerJobResponse>> GetJobsByCompanyAsync(Guid companyId, Guid userId, CancellationToken ct);
    }
}
