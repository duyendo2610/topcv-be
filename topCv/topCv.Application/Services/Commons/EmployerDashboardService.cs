using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Mappings;

namespace topCv.Application.Services.Commons
{
    public sealed class EmployerDashboardService : IEmployerDashboardService
    {
        private readonly IAppDbContext _db;

        public EmployerDashboardService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<List<EmployerCompanyResponse>> GetMyCompaniesAsync(Guid userId, CancellationToken ct)
        {
            var companies = await _db.Companies
                .AsNoTracking()
                .Where(x => x.OwnerUserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);

            return companies.Select(x => x.ToEmployerResponse()).ToList();
        }

        public async Task<List<EmployerJobResponse>> GetJobsByCompanyAsync(Guid companyId, Guid userId,
            CancellationToken ct)
        {
            // check ownership
            var company = await _db.Companies
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.Id == companyId, ct)
                          ?? throw new KeyNotFoundException("Company not found.");

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Not company owner.");

            var jobs = await _db.Jobs
                .AsNoTracking()
                .Where(x => x.CompanyId == companyId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    Job = x,
                    TotalApplications = _db.JobApplications.Count(a => a.JobId == x.Id)
                })
                .ToListAsync(ct);

            return jobs
                .Select(x => x.Job.ToEmployerResponse(x.TotalApplications))
                .ToList();
        }
    }
}