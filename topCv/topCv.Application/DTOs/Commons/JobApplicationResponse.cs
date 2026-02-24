using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class JobApplicationResponse
    {
        public Guid Id { get; init; }
        public Guid JobId { get; init; }
        public string JobTitle { get; init; } = null!;
        public Guid CandidateUserId { get; init; }
        public Guid? ResumeId { get; init; }
        public Guid? ResumeFileId { get; init; }
        public string? ResumeFileUrl { get; init; }
        public string? CoverLetter { get; init; }
        public ApplicationStatus Status { get; init; }
        public DateTime AppliedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}