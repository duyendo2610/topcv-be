using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Mappings;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Services.Commons
{
    public sealed class SavedJobService : ISavedJobService
    {
        private readonly IAppDbContext _db;

        public SavedJobService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task SaveAsync(Guid jobId, Guid userId, CancellationToken ct)
        {
            // 1) Job tồn tại
            var jobExists = await _db.Jobs
                .AsNoTracking()
                .AnyAsync(x => x.Id == jobId, ct);

            if (!jobExists)
                throw new KeyNotFoundException("Job not found.");

            // 2) Không lưu trùng (UserId + JobId)
            var existed = await _db.SavedJobs
                .AnyAsync(x => x.JobId == jobId && x.UserId == userId, ct);

            if (existed)
                return; // idempotent

            // 3) Tạo SavedJob (KHÔNG có Id)
            var entity = new SavedJob
            {
                JobId = jobId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.SavedJobs.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UnsaveAsync(Guid jobId, Guid userId, CancellationToken ct)
        {
            var entity = await _db.SavedJobs
                .FirstOrDefaultAsync(x => x.JobId == jobId && x.UserId == userId, ct);

            if (entity is null)
                return; // idempotent

            _db.SavedJobs.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<SavedJobResponse>> GetMySavedJobsAsync(Guid userId, CancellationToken ct)
        {
            var items = await _db.SavedJobs
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Include(x => x.Job)
                .ThenInclude(j => j.Company)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);

            return items.Select(x => x.ToResponse()).ToList();
        }
    }
}