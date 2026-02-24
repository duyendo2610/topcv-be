using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.Common;
using topCv.Application.DTOs.Obj;
using topCv.Application.DTOs.Object;
using topCv.Application.Interfaces.Obj;
using topCv.Application.Mappings;
using topCv.Domain.Entities.Obj;

namespace topCv.Application.Services.Obj
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
