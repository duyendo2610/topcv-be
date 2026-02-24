using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Mappings;
using topCv.Domain.Entities.Commons;
using topCv.Domain.Enums;

namespace topCv.Application.Services.Commons
{
    public sealed class JobApplicationService : IJobApplicationService
    {
        private readonly IAppDbContext _db;
        private readonly INotificationService _noti;

        public JobApplicationService(
            IAppDbContext db,
            INotificationService noti)
        {
            _db = db;
            _noti = noti;
        }

        public async Task<JobApplicationResponse> ApplyAsync(CreateJobApplicationRequest req, Guid userId,
            CancellationToken ct)
        {
            var job = await _db.Jobs
                          .AsNoTracking()
                          .FirstOrDefaultAsync(x => x.Id == req.JobId, ct)
                      ?? throw new KeyNotFoundException("Job not found.");

            if (job.Status != JobStatus.Published) // 1 = Published
                throw new InvalidOperationException("Job is not published.");

            if (job.DeadlineAt is DateTime d && d <= DateTime.UtcNow)
                throw new InvalidOperationException("Job deadline has passed.");

            var existed = await _db.JobApplications
                .AsNoTracking()
                .AnyAsync(x => x.JobId == req.JobId && x.CandidateUserId == userId, ct);

            if (existed)
                throw new InvalidOperationException("You already applied for this job.");

            if (req.ResumeId is Guid resumeId)
            {
                var resume = await _db.Resumes
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(x => x.Id == resumeId, ct)
                             ?? throw new KeyNotFoundException("Resume not found.");

                if (resume.UserId != userId)
                    throw new UnauthorizedAccessException("Not your resume.");

                if (req.ResumeFileId is Guid fileId)
                {
                    var fileOk = await _db.ResumeFiles
                        .AsNoTracking()
                        .AnyAsync(x => x.Id == fileId && x.ResumeId == resumeId, ct);

                    if (!fileOk)
                        throw new InvalidOperationException("ResumeFileId not found in this resume.");
                }
            }
            else
            {
                if (req.ResumeFileId is not null)
                    throw new InvalidOperationException("ResumeId is required when ResumeFileId is provided.");
            }

            var entity = req.ToEntity(userId);
            _db.JobApplications.Add(entity);
            await _db.SaveChangesAsync(ct);

            await _noti.CreateAsync(new CreateNotificationRequest
            {
                UserId = job.Company.OwnerUserId, // Employer nhận
                Type = NotificationType.Other,
                Title = "New application",
                Body = $"A candidate applied for '{job.Title}'."
            }, ct);

            var saved = await LoadForResponse(entity.Id, ct);
            return saved.ToResponse();
        }

        public async Task<List<JobApplicationResponse>> GetByJobAsync(Guid jobId, Guid employerUserId,
            CancellationToken ct)
        {
            var job = await _db.Jobs
                          .AsNoTracking()
                          .FirstOrDefaultAsync(x => x.Id == jobId, ct)
                      ?? throw new KeyNotFoundException("Job not found.");

            // 2) Employer must own company of this job
            var company = await _db.Companies
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.Id == job.CompanyId, ct)
                          ?? throw new KeyNotFoundException("Company not found.");

            if (company.OwnerUserId != employerUserId)
                throw new UnauthorizedAccessException("Not company owner.");

            // 3) Load applications
            var items = await _db.JobApplications
                .AsNoTracking()
                .Where(x => x.JobId == jobId)
                .Include(x => x.Job)
                .Include(x => x.ResumeFile)
                .OrderByDescending(x => x.AppliedAt)
                .ToListAsync(ct);

            return items.Select(x => x.ToResponse()).ToList();
        }

        public async Task<List<JobApplicationResponse>> GetMyApplicationsAsync(Guid userId, CancellationToken ct)
        {
            var items = await _db.JobApplications
                .AsNoTracking()
                .Where(x => x.CandidateUserId == userId)
                .Include(x => x.Job)
                .Include(x => x.ResumeFile)
                .OrderByDescending(x => x.AppliedAt)
                .ToListAsync(ct);

            return items.Select(x => x.ToResponse()).ToList();
        }

        public async Task<JobApplicationResponse> UpdateMyApplicationAsync(Guid applicationId,
            UpdateJobApplicationRequest req, Guid userId, CancellationToken ct)
        {
            var entity = await _db.JobApplications
                             .FirstOrDefaultAsync(x => x.Id == applicationId && x.CandidateUserId == userId, ct)
                         ?? throw new KeyNotFoundException("Application not found.");

            // Validate resume ownership nếu có sửa resume/file
            if (req.ResumeId is Guid resumeId)
            {
                var resume = await _db.Resumes
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(x => x.Id == resumeId, ct)
                             ?? throw new KeyNotFoundException("Resume not found.");

                if (resume.UserId != userId)
                    throw new UnauthorizedAccessException("Not your resume.");

                if (req.ResumeFileId is Guid fileId)
                {
                    var fileOk = await _db.ResumeFiles
                        .AsNoTracking()
                        .AnyAsync(x => x.Id == fileId && x.ResumeId == resumeId, ct);

                    if (!fileOk)
                        throw new InvalidOperationException("ResumeFileId not found in this resume.");
                }
            }
            else
            {
                if (req.ResumeFileId is not null)
                    throw new InvalidOperationException("ResumeId is required when ResumeFileId is provided.");
            }

            // Apply update + UpdatedAt
            req.ApplyTo(entity);
            await _db.SaveChangesAsync(ct);

            var saved = await LoadForResponse(entity.Id, ct);
            return saved.ToResponse();
        }

        public async Task<JobApplicationResponse> UpdateStatusAsync(Guid applicationId,
            UpdateJobApplicationStatusRequest req, Guid employerUserId, CancellationToken ct)
        {
            // Load application + job (để check quyền owner)
            var entity = await _db.JobApplications
                             .Include(x => x.Job)
                             .FirstOrDefaultAsync(x => x.Id == applicationId, ct)
                         ?? throw new KeyNotFoundException("Application not found.");

            // check owner via job.CompanyId
            var company = await _db.Companies
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.Id == entity.Job.CompanyId, ct)
                          ?? throw new KeyNotFoundException("Company not found.");

            if (company.OwnerUserId != employerUserId)
                throw new UnauthorizedAccessException("Not company owner.");

            // Apply status + UpdatedAt
            req.ApplyStatus(entity);
            await _db.SaveChangesAsync(ct);

            await _noti.CreateAsync(new CreateNotificationRequest
            {
                UserId = entity.CandidateUserId, // Candidate nhận
                Type = NotificationType.ApplyStatusChanged,
                Title = "Application updated",
                Body = $"Your application for '{entity.Job.Title}' was updated to {entity.Status}."
            }, ct);

            var saved = await LoadForResponse(entity.Id, ct);
            return saved.ToResponse();
        }

        private async Task<JobApplication> LoadForResponse(Guid id, CancellationToken ct)
        {
            return await _db.JobApplications
                .AsNoTracking()
                .Include(x => x.Job)
                .Include(x => x.ResumeFile)
                .FirstAsync(x => x.Id == id, ct);
        }
    }
}