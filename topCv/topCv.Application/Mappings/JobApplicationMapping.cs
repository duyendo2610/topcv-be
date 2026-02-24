using topCv.Application.DTOs.Commons;
using topCv.Domain.Entities.Commons;
using topCv.Domain.Enums;

namespace topCv.Application.Mappings
{
    public static class ApplicationMapping
    {
        // CREATE
        public static JobApplication ToEntity(this CreateJobApplicationRequest req, Guid candidateUserId)
            => new()
            {
                Id = Guid.NewGuid(),
                JobId = req.JobId,
                CandidateUserId = candidateUserId,
                ResumeId = req.ResumeId,
                ResumeFileId = req.ResumeFileId,
                CoverLetter = req.CoverLetter,
                Status = ApplicationStatus.Submitted,
                AppliedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

        // READ
        public static JobApplicationResponse ToResponse(this JobApplication entity)
            => new()
            {
                Id = entity.Id,
                JobId = entity.JobId,
                JobTitle = entity.Job?.Title ?? string.Empty,
                CandidateUserId = entity.CandidateUserId,
                ResumeId = entity.ResumeId,
                ResumeFileId = entity.ResumeFileId,
                ResumeFileUrl = entity.ResumeFile?.FileUrl,
                CoverLetter = entity.CoverLetter,
                Status = entity.Status,
                AppliedAt = entity.AppliedAt,
                UpdatedAt = entity.UpdatedAt
            };

        //UPDATE CONTENT(Candidate)
        public static void ApplyTo(this UpdateJobApplicationRequest req, JobApplication entity)
        {
            entity.ResumeFileId = req.ResumeFileId;
            entity.ResumeId = req.ResumeId;
            entity.CoverLetter = req.CoverLetter;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        // UPDATE STATUS (Employer)
        public static void ApplyStatus(this UpdateJobApplicationStatusRequest req, JobApplication entity)
        {
            entity.Status = req.Status;
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}