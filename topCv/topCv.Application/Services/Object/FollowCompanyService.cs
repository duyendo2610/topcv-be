using topCv.Application.Common;
using topCv.Application.DTOs.Obj;
using topCv.Domain.Entities.Obj;
using Microsoft.EntityFrameworkCore;
using topCv.Application.Interfaces.Obj;
using topCv.Application.Mappings;

namespace topCv.Application.Services.Obj
{
    public sealed class FollowCompanyService : IFollowCompanyService
    {
        private readonly IAppDbContext _db;

        public FollowCompanyService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task FollowAsync(Guid companyId, Guid userId, CancellationToken ct)
        {
            // Company must exist
            var companyExists = await _db.Companies
                .AsNoTracking()
                .AnyAsync(x => x.Id == companyId, ct);

            if (!companyExists)
                throw new KeyNotFoundException("Company not found.");

            // Prevent duplicate (UserId + CompanyId)
            var existed = await _db.FollowCompanies
                .AnyAsync(x => x.UserId == userId && x.CompanyId == companyId, ct);

            if (existed)
                return; 

            var entity = new FollowCompany
            {
                UserId = userId,
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow
            };

            _db.FollowCompanies.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UnfollowAsync(Guid companyId, Guid userId, CancellationToken ct)
        {
            var entity = await _db.FollowCompanies
                .FirstOrDefaultAsync(x => x.UserId == userId && x.CompanyId == companyId, ct);

            if (entity is null)
                return;

            _db.FollowCompanies.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<FollowCompanyResponse>> GetMyFollowedAsync(Guid userId, CancellationToken ct)
        {
            var items = await _db.FollowCompanies
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Include(x => x.Company)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);

            return items.Select(x => x.ToResponse()).ToList();
        }
    }
}
