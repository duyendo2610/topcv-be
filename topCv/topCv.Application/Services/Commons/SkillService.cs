using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Mappings;

namespace topCv.Application.Services.Commons
{
    public sealed class SkillService : ISkillService
    {
        private readonly IAppDbContext _db;

        public SkillService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<List<IdNameResponse>> GetAllAsync(CancellationToken ct)
        {
            var items = await _db.Skills
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync(ct);

            return items.Select(x => x.ToResponse()).ToList();
        }

        public async Task<IdNameResponse> CreateAsync(CreateNameRequest req, CancellationToken ct)
        {
            var name = req.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên không được để trống.");

            var exists = await _db.Skills
                .AsNoTracking()
                .AnyAsync(x => x.Name == name, ct);

            if (exists)
                throw new InvalidOperationException("Kỹ năng đã tồn tại.");

            var entity = new CreateNameRequest { Name = name }.ToSkill();

            _db.Skills.Add(entity);
            await _db.SaveChangesAsync(ct);

            return entity.ToResponse();
        }

        public async Task<IdNameResponse> UpdateAsync(int id, UpdateNameRequest req, CancellationToken ct)
        {
            var entity = await _db.Skills
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (entity is null)
                throw new KeyNotFoundException("Không tìm thấy kỹ năng.");

            var name = req.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên không được để trống.");

            var exists = await _db.Skills
                .AsNoTracking()
                .AnyAsync(x => x.Id != id && x.Name == name, ct);

            if (exists)
                throw new InvalidOperationException("Tên kỹ năng đã tồn tại.");

            new UpdateNameRequest { Name = name }.ApplyTo(entity);
            await _db.SaveChangesAsync(ct);

            return entity.ToResponse();
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await _db.Skills
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (entity is null)
                throw new KeyNotFoundException("Không tìm thấy kỹ năng.");

            _db.Skills.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
