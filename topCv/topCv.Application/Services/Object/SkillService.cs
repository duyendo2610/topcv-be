using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.Common;
using topCv.Application.DTOs.Obj;
using topCv.Application.Interfaces.Obj;
using topCv.Application.Mappings;
using topCv.Domain.Entities.Obj;

namespace topCv.Application.Services.Obj
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
                throw new ArgumentException("Name is required.");

            var exists = await _db.Skills
                .AsNoTracking()
                .AnyAsync(x => x.Name == name, ct);

            if (exists)
                throw new InvalidOperationException("Skill already exists.");

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
                throw new KeyNotFoundException("Skill not found.");

            var name = req.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");

            var exists = await _db.Skills
                .AsNoTracking()
                .AnyAsync(x => x.Id != id && x.Name == name, ct);

            if (exists)
                throw new InvalidOperationException("Skill name already exists.");

            new UpdateNameRequest { Name = name }.ApplyTo(entity);
            await _db.SaveChangesAsync(ct);

            return entity.ToResponse();
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await _db.Skills
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (entity is null)
                throw new KeyNotFoundException("Skill not found.");

            _db.Skills.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
