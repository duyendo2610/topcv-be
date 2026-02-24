using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Mappings;

namespace topCv.Application.Services.Commons
{
    public sealed class ProvinceService : IProvinceService
    {
        private readonly IAppDbContext _db;

        public ProvinceService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ProvinceResponse>> GetAllAsync(CancellationToken ct)
        {
            var provinces = await _db.Provinces
                .Include(p => p.Wards)
                .AsNoTracking()
                .ToListAsync(ct);

            return provinces
                .Select(p => p.ToResponse())
                .ToList();
        }

        public async Task<List<ProvinceResponse>> SearchAsync(string keyword, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new();

            keyword = keyword.Trim().ToLower();

            var provinces = await _db.Provinces
                .AsNoTracking()
                .Where(p =>
                    p.Codename.StartsWith(keyword) ||
                    p.Name.Replace("Tỉnh ", "")
                        .Replace("Thành phố ", "")
                        .StartsWith(keyword))
                .OrderBy(p => p.Name)
                .Take(20)
                .ToListAsync(ct);

            return provinces.Select(p => p.ToResponse()).ToList();
        }
    }
}