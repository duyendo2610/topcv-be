using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Mappings;

namespace topCv.Application.Services.Commons
{
    public sealed class WardService : IWardService
    {
        private readonly IAppDbContext _dbContext;

        public WardService(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<WardResponse>> GetAllAsync()
        {
            var wards = await _dbContext.Wards
                .AsNoTracking()
                .ToListAsync();

            return wards.Select(w => w.ToResponse()).ToList();
        }

        public async Task<List<WardResponse>> GetByProvinceIdAsync(int provinceId)
        {
            var wards = await _dbContext.Wards
                .Where(w => w.ProvinceId == provinceId)
                .AsNoTracking()
                .ToListAsync();

            return wards.Select(w => w.ToResponse()).ToList();
        }

        public async Task<List<WardResponse>> SearchAsync(string keyword, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new();

            keyword = keyword.Trim().ToLower();

            var wards = await _dbContext.Wards
                .AsNoTracking()
                .Where(w =>
                    w.ShortCodename.StartsWith(keyword) ||
                    w.Name.Replace("Phường ", "")
                        .Replace("Xã ", "")
                        .StartsWith(keyword))
                .OrderBy(w => w.Name)
                .Take(20)
                .ToListAsync(ct);

            return wards.Select(w => w.ToResponse()).ToList();
        }
    }
}