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
    public sealed class CityService : ICityService
    {
        private readonly IAppDbContext _db;

        public CityService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<List<IdNameResponse>> GetAllAsync(CancellationToken ct)
        {
            var items = await _db.Cities
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

            var exists = await _db.Cities
                .AsNoTracking()
                .AnyAsync(x => x.Name == name, ct);

            if (exists)
                throw new InvalidOperationException("City already exists.");

            // mapping extension
            var entity = new CreateNameRequest { Name = name }.ToCity();

            _db.Cities.Add(entity);
            await _db.SaveChangesAsync(ct);

            return entity.ToResponse();
        }

        public async Task<IdNameResponse> UpdateAsync(int id, UpdateNameRequest req, CancellationToken ct)
        {
            var entity = await _db.Cities
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (entity is null)
                throw new KeyNotFoundException("City not found.");

            var name = req.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");

            var exists = await _db.Cities
                .AsNoTracking()
                .AnyAsync(x => x.Id != id && x.Name == name, ct);

            if (exists)
                throw new InvalidOperationException("City name already exists.");

            new UpdateNameRequest { Name = name }.ApplyTo(entity);
            await _db.SaveChangesAsync(ct);

            return entity.ToResponse();
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await _db.Cities
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (entity is null)
                throw new KeyNotFoundException("City not found.");

            _db.Cities.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }

}
