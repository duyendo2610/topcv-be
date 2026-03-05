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
                          ?? throw new KeyNotFoundException("Không tìm thấy công ty.");

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Bạn không phải chủ sở hữu công ty.");

            var title = req.Title.Trim();
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Tiêu đề không được để trống.");

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
                      ?? throw new KeyNotFoundException("Không tìm thấy tin tuyển dụng.");

            // owner check via company owner
            var company = await _db.Companies
                .AsNoTracking()
                .FirstAsync(x => x.Id == job.CompanyId, ct);

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Bạn không phải chủ sở hữu công ty.");

            var title = req.Title.Trim();
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Tiêu đề không được để trống.");

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
                      ?? throw new KeyNotFoundException("Không tìm thấy tin tuyển dụng.");

            var company = await _db.Companies.AsNoTracking()
                .FirstAsync(x => x.Id == job.CompanyId, ct);

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Bạn không phải chủ sở hữu công ty.");

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
                      ?? throw new KeyNotFoundException("Không tìm thấy tin tuyển dụng.");

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

            // Salary filtering (overlap) with currency normalization to VND
            if (req.SalaryMin is decimal || req.SalaryMax is decimal)
            {
                const decimal usdToVndRate = 26000m;

                if (req.SalaryMin is decimal salaryMin)
                {
                    baseQuery = baseQuery.Where(x =>
                        (x.SalaryMax ?? x.SalaryMin ?? 0) *
                        (x.Currency == "USD" ? usdToVndRate : 1m) >= salaryMin);
                }

                if (req.SalaryMax is decimal salaryMax)
                {
                    baseQuery = baseQuery.Where(x =>
                        (x.SalaryMin ?? x.SalaryMax ?? 0) *
                        (x.Currency == "USD" ? usdToVndRate : 1m) <= salaryMax);
                }
            }

            if (req.ExpMin is int expMin)
            {
                baseQuery = baseQuery.Where(x => (x.ExpMin ?? 0) >= expMin);
            }

            if (req.ExpMax is int expMax)
            {
                baseQuery = baseQuery.Where(x => ((x.ExpMax ?? x.ExpMin) ?? 0) <= expMax);
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
                      ?? throw new KeyNotFoundException("Không tìm thấy tin tuyển dụng.");

            var company = await _db.Companies.AsNoTracking()
                .FirstAsync(x => x.Id == job.CompanyId, ct);

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Bạn không phải chủ sở hữu công ty.");

            if (job.Status == JobStatus.Published)
                return;

            job.Status = JobStatus.Draft;
            job.SubmittedAtUtc ??= DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        public async Task CloseAsync(Guid id, Guid userId, CancellationToken ct)
        {
            var job = await _db.Jobs.FirstOrDefaultAsync(x => x.Id == id, ct)
                      ?? throw new KeyNotFoundException("Không tìm thấy tin tuyển dụng.");

            var company = await _db.Companies.AsNoTracking()
                .FirstAsync(x => x.Id == job.CompanyId, ct);

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Bạn không phải chủ sở hữu công ty.");

            job.Status = JobStatus.Closed; // Closed
            job.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<AdminJobApprovalResponse>> GetPendingApprovalsAsync(CancellationToken ct)
        {
            return await _db.Jobs
                .AsNoTracking()
                .Include(x => x.Company)
                .Where(x => x.Status == JobStatus.Draft && x.SubmittedAtUtc != null)
                .OrderByDescending(x => x.SubmittedAtUtc)
                .Select(x => new AdminJobApprovalResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.Name,
                    Status = x.Status,
                    CreatedAtUtc = x.CreatedAt,
                    SubmittedAtUtc = x.SubmittedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task ApproveAsync(Guid id, Guid adminUserId, CancellationToken ct)
        {
            var job = await _db.Jobs
                          .FirstOrDefaultAsync(x => x.Id == id, ct)
                      ?? throw new KeyNotFoundException("Không tìm thấy tin tuyển dụng.");

            if (job.Status != JobStatus.Draft || job.SubmittedAtUtc == null)
                throw new InvalidOperationException("Tin tuyển dụng không ở trạng thái chờ duyệt.");

            job.Status = JobStatus.Published;
            job.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        public async Task RejectAsync(Guid id, Guid adminUserId, CancellationToken ct)
        {
            var job = await _db.Jobs
                          .FirstOrDefaultAsync(x => x.Id == id, ct)
                      ?? throw new KeyNotFoundException("Không tìm thấy tin tuyển dụng.");

            if (job.Status != JobStatus.Draft || job.SubmittedAtUtc == null)
                throw new InvalidOperationException("Tin tuyển dụng không ở trạng thái chờ duyệt.");

            job.Status = JobStatus.Closed;
            job.UpdatedAt = DateTime.UtcNow;
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
                    throw new InvalidOperationException("Một số SkillId không hợp lệ.");
            }

            if (categoryIds.Count > 0)
            {
                var distinct = categoryIds.Distinct().ToList();
                var found = await _db.Categories.AsNoTracking()
                    .CountAsync(x => distinct.Contains(x.Id), ct);

                if (found != distinct.Count)
                    throw new InvalidOperationException("Một số CategoryId không hợp lệ.");
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
