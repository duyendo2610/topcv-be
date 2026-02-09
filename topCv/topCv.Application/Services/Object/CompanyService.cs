using topCv.Application.Common;
using topCv.Application.DTOs.Obj;
using topCv.Application.Interfaces.Obj;
using Microsoft.EntityFrameworkCore;
using topCv.Application.Mappings;

namespace topCv.Application.Services.Obj
{
    public sealed class CompanyService : ICompanyService
    {
        private readonly IAppDbContext _db;

        public CompanyService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<CompanyResponse> CreateAsync(CreateCompanyRequest req,Guid userId,CancellationToken ct)
        {
            var name = req.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Company name is required.");

            var duplicated = await _db.Companies.AsNoTracking()
                .AnyAsync(x => x.OwnerUserId == userId && x.Name == name, ct);
            if (duplicated) throw new InvalidOperationException("You already created a company with the same name.");

            var company = req.ToCompany(userId);

            _db.Companies.Add(company);
            await _db.SaveChangesAsync(ct);
            // muốn có CityName thì query lại include City (vì vừa add chưa include navigation)
            var saved = await _db.Companies
                .Include(x => x.Province)
                .AsNoTracking()
                .FirstAsync(x => x.Id == company.Id, ct);

            return saved.ToResponse();
        }

        public async Task<CompanyResponse> UpdateAsync(
            Guid id,
            UpdateCompanyRequest req,
            Guid userId,
            CancellationToken ct)
        {
            var company = await _db.Companies
                .Include(x => x.Province)
                .FirstOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new KeyNotFoundException("Company not found.");

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Not company owner.");

            var name = req.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Company name is required.");

            var duplicated = await _db.Companies.AsNoTracking()
                .AnyAsync(x => x.OwnerUserId == userId && x.Id != id && x.Name == name, ct);
            if (duplicated) throw new InvalidOperationException("Company name already exists in your companies.");

            req.ApplyTo(company);

            await _db.SaveChangesAsync(ct);

            // refresh city navigation (nếu CityId thay đổi)
            var updated = await _db.Companies
                .Include(x => x.Province)
                .AsNoTracking()
                .FirstAsync(x => x.Id == id, ct);

            return updated.ToResponse();
        }

        public async Task<CompanyResponse> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var company = await _db.Companies
                .Include(x => x.Province)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new KeyNotFoundException("Company not found.");

            return company.ToResponse();
        }

        public async Task<List<CompanyResponse>> GetAllAsync(CancellationToken ct)
        {
            var companies = await _db.Companies
                .Include(x => x.Province)
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt) // nếu entity có CreatedAt
                .ToListAsync(ct);

            return companies.Select(x => x.ToResponse()).ToList();
        }

        public async Task<List<CompanyResponse>> GetMyCompaniesAsync(Guid userId, CancellationToken ct)
        {
            var companies = await _db.Companies
                .Include(x => x.Province)
                .AsNoTracking()
                .Where(x => x.OwnerUserId == userId)
                .OrderByDescending(x => x.CreatedAt) // nếu có
                .ToListAsync(ct);

            return companies.Select(x => x.ToResponse()).ToList();
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken ct)
        {
            var company = await _db.Companies
                .FirstOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new KeyNotFoundException("Company not found.");

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Not company owner.");

            _db.Companies.Remove(company);
            await _db.SaveChangesAsync(ct);
        }
    }

}
