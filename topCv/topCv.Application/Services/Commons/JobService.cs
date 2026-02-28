using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Mappings;
using topCv.Domain.Entities.Commons;
using topCv.Domain.Enums;

namespace topCv.Application.Services.Commons
{
    public sealed class JobService : IJobService
    {
        private readonly IAppDbContext _db;

        public JobService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<JobResponse> CreateAsync(CreateJobRequest req, Guid userId, CancellationToken ct)
        {
            // company must exist & belong to user
            var company = await _db.Companies
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.Id == req.CompanyId, ct)
                          ?? throw new KeyNotFoundException("Company not found.");

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Not company owner.");

            var title = req.Title.Trim();
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");

            // validate master ids
            await ValidateSkillsAndCategories(req.SkillIds, req.CategoryIds, ct);

            var job = req.ToJob(userId);

            _db.Jobs.Add(job);

            // join rows
            var skillIds = req.SkillIds.Distinct().ToList();
            var categoryIds = req.CategoryIds.Distinct().ToList();

            if (skillIds.Count > 0)
                _db.JobSkills.AddRange(skillIds.Select(sid => new JobSkill { JobId = job.Id, SkillId = sid }));

            if (categoryIds.Count > 0)
                _db.JobCategories.AddRange(categoryIds.Select(cid => new JobCategory
                    { JobId = job.Id, CategoryId = cid }));

            await _db.SaveChangesAsync(ct);

            // load navigation for response
            var saved = await LoadJobForResponse(job.Id, ct);
            return saved.ToResponse();
        }

        public async Task<JobResponse> UpdateAsync(Guid id, UpdateJobRequest req, Guid userId, CancellationToken ct)
        {
            var job = await _db.Jobs
                          .Include(x => x.Company)
                          .FirstOrDefaultAsync(x => x.Id == id, ct)
                      ?? throw new KeyNotFoundException("Job not found.");

            // owner check via company owner
            var company = await _db.Companies
                .AsNoTracking()
                .FirstAsync(x => x.Id == job.CompanyId, ct);

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Not company owner.");

            var title = req.Title.Trim();
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");

            await ValidateSkillsAndCategories(req.SkillIds, req.CategoryIds, ct);

            req.ApplyTo(job);

            // sync joins (simple & safe for MVP)
            var oldSkills = await _db.JobSkills.Where(x => x.JobId == id).ToListAsync(ct);
            var oldCats = await _db.JobCategories.Where(x => x.JobId == id).ToListAsync(ct);

            _db.JobSkills.RemoveRange(oldSkills);
            _db.JobCategories.RemoveRange(oldCats);

            var skillIds = req.SkillIds.Distinct().ToList();
            var categoryIds = req.CategoryIds.Distinct().ToList();

            if (skillIds.Count > 0)
                _db.JobSkills.AddRange(skillIds.Select(sid => new JobSkill { JobId = id, SkillId = sid }));

            if (categoryIds.Count > 0)
                _db.JobCategories.AddRange(categoryIds.Select(cid => new JobCategory { JobId = id, CategoryId = cid }));

            await _db.SaveChangesAsync(ct);

            var updated = await LoadJobForResponse(id, ct);
            return updated.ToResponse();
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken ct)
        {
            var job = await _db.Jobs.FirstOrDefaultAsync(x => x.Id == id, ct)
                      ?? throw new KeyNotFoundException("Job not found.");

            var company = await _db.Companies.AsNoTracking()
                .FirstAsync(x => x.Id == job.CompanyId, ct);

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Not company owner.");

            // remove joins first (safe)
            var js = await _db.JobSkills.Where(x => x.JobId == id).ToListAsync(ct);
            var jc = await _db.JobCategories.Where(x => x.JobId == id).ToListAsync(ct);

            _db.JobSkills.RemoveRange(js);
            _db.JobCategories.RemoveRange(jc);

            _db.Jobs.Remove(job);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<JobResponse> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var job = await LoadJobForResponse(id, ct)
                      ?? throw new KeyNotFoundException("Job not found.");

            return job.ToResponse();
        }

        public async Task<PagedResult<JobResponse>> SearchAsync(JobQueryRequest req, CancellationToken ct)
        {
            var page = req.Page < 1 ? 1 : req.Page;
            var pageSize = req.PageSize <= 0 ? 20 : req.PageSize;

            var baseQuery = _db.Jobs
                .AsNoTracking()
                .AsQueryable();

            // only Published for public list? (tuỳ bạn)
            // query = query.Where(x => x.Status == 1);

            if (!string.IsNullOrWhiteSpace(req.Keyword))
            {
                var kw = req.Keyword.Trim();
                baseQuery = baseQuery.Where(x => x.Title.Contains(kw) || x.Company.Name.Contains(kw));
            }

            if (req.CityId is int cityId)
                baseQuery = baseQuery.Where(x => x.CityId == cityId);

            if (req.Level is JobLevel level)
                baseQuery = baseQuery.Where(x => x.Level == level);

            if (req.JobType is JobType jobType)
                baseQuery = baseQuery.Where(x => x.JobType == jobType);

            if (req.SkillIds is { Count: > 0 })
            {
                var skillSet = req.SkillIds.Distinct().ToList();
                baseQuery = baseQuery.Where(j => j.JobSkills.Any(s => skillSet.Contains(s.SkillId)));
            }

            if (req.CategoryIds is { Count: > 0 })
            {
                var catSet = req.CategoryIds.Distinct().ToList();
                baseQuery = baseQuery.Where(j => j.JobCategories.Any(c => catSet.Contains(c.CategoryId)));
            }

            var totalItems = await baseQuery.LongCountAsync(ct);

            var items = await baseQuery
                .Include(x => x.Company)
                .Include(x => x.Province)
                .Include(x => x.JobSkills)
                .Include(x => x.JobCategories)
                .OrderByDescending(x => x.CreatedAt) // nếu không có CreatedAt thì đổi sang Id
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<JobResponse>
            {
                Items = items.Select(x => x.ToResponse()).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }

        public async Task PublishAsync(Guid id, Guid userId, CancellationToken ct)
        {
            var job = await _db.Jobs.FirstOrDefaultAsync(x => x.Id == id, ct)
                      ?? throw new KeyNotFoundException("Job not found.");

            var company = await _db.Companies.AsNoTracking()
                .FirstAsync(x => x.Id == job.CompanyId, ct);

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Not company owner.");

            job.Status = JobStatus.Published; // Published
            await _db.SaveChangesAsync(ct);
        }

        public async Task CloseAsync(Guid id, Guid userId, CancellationToken ct)
        {
            var job = await _db.Jobs.FirstOrDefaultAsync(x => x.Id == id, ct)
                      ?? throw new KeyNotFoundException("Job not found.");

            var company = await _db.Companies.AsNoTracking()
                .FirstAsync(x => x.Id == job.CompanyId, ct);

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Not company owner.");

            job.Status = JobStatus.Closed; // Closed
            await _db.SaveChangesAsync(ct);
        }

        private async Task ValidateSkillsAndCategories(List<int> skillIds, List<int> categoryIds, CancellationToken ct)
        {
            if (skillIds.Count > 0)
            {
                var distinct = skillIds.Distinct().ToList();
                var found = await _db.Skills.AsNoTracking()
                    .CountAsync(x => distinct.Contains(x.Id), ct);

                if (found != distinct.Count)
                    throw new InvalidOperationException("Some SkillIds are invalid.");
            }

            if (categoryIds.Count > 0)
            {
                var distinct = categoryIds.Distinct().ToList();
                var found = await _db.Categories.AsNoTracking()
                    .CountAsync(x => distinct.Contains(x.Id), ct);

                if (found != distinct.Count)
                    throw new InvalidOperationException("Some CategoryIds are invalid.");
            }
        }

        private async Task<Job?> LoadJobForResponse(Guid id, CancellationToken ct)
        {
            return await _db.Jobs
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.Province)
                .Include(x => x.JobSkills)
                .Include(x => x.JobCategories)
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }
    }
}
