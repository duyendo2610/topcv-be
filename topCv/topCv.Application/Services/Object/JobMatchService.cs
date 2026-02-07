using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.Common;
using topCv.Application.DTOs.Obj;
using topCv.Application.Interfaces.Obj;
using topCv.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using topCv.Domain.Entities.Obj;

namespace topCv.Application.Services.Obj
{
    public sealed class JobMatchService : IJobMatchService
    {
        private readonly IAppDbContext _db;
        private readonly INotificationService _noti;

        public JobMatchService(IAppDbContext db, INotificationService noti)
        {
            _db = db;
            _noti = noti;
        }

        public async Task<List<JobMatchResponse>> GetMyMatchesAsync(Guid userId, int take, CancellationToken ct)
        {
            if (take <= 0) take = 20;
            if (take > 50) take = 50;

            var profile = await _db.CandidateProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId, ct);

            if (profile is null)
                throw new InvalidOperationException("Candidate profile not found.");

            // base query: Published + chưa quá deadline
            // NOTE: Status Published = 1 (bạn đang dùng int enum)
            var now = DateTime.UtcNow;

            var jobs = await _db.Jobs
                .AsNoTracking()
                .Where(j => j.Status == JobStatus.Published)
                .Where(j => j.DeadlineAt == null || j.DeadlineAt > now)
                .Include(j => j.Company)
                .Include(j => j.City)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync(ct);

            int ScoreJob(Job j)
            {
                var score = 0;

                // City match
                if (profile.CityId is int cityId)
                {
                    if (j.CityId == cityId) score += 3;
                    else score -= 1; // không đúng city thì giảm nhẹ
                }

                // Experience match
                if (profile.YearsOfExperience is int yoe)
                {
                    // nếu job không set exp range -> cho điểm nhỏ
                    if (j.ExpMin is null && j.ExpMax is null) score += 1;
                    else
                    {
                        var okMin = j.ExpMin is null || yoe >= j.ExpMin.Value;
                        var okMax = j.ExpMax is null || yoe <= j.ExpMax.Value;
                        if (okMin && okMax) score += 3;
                        else score -= 1;
                    }
                }

                return score;
            }

            var ranked = jobs
                .Select(j => new { Job = j, Score = ScoreJob(j) })
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Job.CreatedAt)
                .Take(take)
                .Select(x => new JobMatchResponse
                {
                    JobId = x.Job.Id,
                    Title = x.Job.Title,

                    CompanyId = x.Job.CompanyId,
                    CompanyName = x.Job.Company?.Name ?? string.Empty,

                    CityId = x.Job.CityId,
                    CityName = x.Job.City != null ? x.Job.City.Name : null,

                    SalaryMin = x.Job.SalaryMin,
                    SalaryMax = x.Job.SalaryMax,

                    CreatedAt = x.Job.CreatedAt,
                    Score = x.Score
                })
                .ToList();

            return ranked;
        }

        public async Task<int> NotifyMyMatchesAsync(Guid userId, int take, CancellationToken ct)
        {
            var matches = await GetMyMatchesAsync(userId, take, ct);
            if (matches.Count == 0) return 0;

            // Chống spam: vì Notification của bạn không có Data/JobId riêng,
            // mình gắn marker vào Body: "[JobId:xxxx]"
            // rồi check trùng bằng contains marker.
            var markers = matches.Select(m => $"[JobId:{m.JobId}]").ToList();

            var existedMarkers = await _db.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId && n.Type == NotificationType.JobMatched)
                .Where(n => n.Body != null)
                .Select(n => n.Body!)
                .ToListAsync(ct);

            var existedSet = new HashSet<string>(
                existedMarkers.Where(b => markers.Any(m => b.Contains(m)))
            );

            var created = 0;

            foreach (var m in matches)
            {
                var marker = $"[JobId:{m.JobId}]";
                var already = existedSet.Any(x => x.Contains(marker));
                if (already) continue;

                await _noti.CreateAsync(new CreateNotificationRequest
                {
                    UserId = userId,
                    Type = NotificationType.JobMatched,
                    Title = "Job matched",
                    Body = $"{marker} You may like: '{m.Title}' at '{m.CompanyName}'."
                }, ct);

                created++;
            }

            return created;
        }
    }
}
