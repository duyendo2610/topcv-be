using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Mappings;

namespace topCv.Application.Services.Commons
{
    public sealed class CompanyService : ICompanyService
    {
        private readonly IAppDbContext _db;

        public CompanyService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<CompanyResponse> CreateAsync(CreateCompanyRequest req, Guid userId, CancellationToken ct)
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

        public async Task<PagedResult<CompanyResponse>> GetAllAsync(CompanyQueryRequest req, CancellationToken ct)
        {
            var page = req.Page < 1 ? 1 : req.Page;
            var pageSize = req.PageSize <= 0 ? 20 : req.PageSize;

            var baseQuery = _db.Companies
                .AsNoTracking()
                .AsQueryable();

            var totalItems = await baseQuery.LongCountAsync(ct);

            var companies = await baseQuery
                .Include(x => x.Province)
                .OrderByDescending(x => x.CreatedAt) // nếu entity có CreatedAt
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<CompanyResponse>
            {
                Items = companies.Select(x => x.ToResponse()).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }

        public async Task<PagedResult<CompanyResponse>> GetMyCompaniesAsync(Guid userId, CompanyQueryRequest req, CancellationToken ct)
        {
            var page = req.Page < 1 ? 1 : req.Page;
            var pageSize = req.PageSize <= 0 ? 20 : req.PageSize;

            var baseQuery = _db.Companies
                .AsNoTracking()
                .Where(x => x.OwnerUserId == userId)
                .AsQueryable();

            var totalItems = await baseQuery.LongCountAsync(ct);

            var companies = await baseQuery
                .Include(x => x.Province)
                .OrderByDescending(x => x.CreatedAt) // nếu có
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<CompanyResponse>
            {
                Items = companies.Select(x => x.ToResponse()).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
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
