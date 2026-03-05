using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class AdminJobApprovalResponse
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = null!;
        public Guid CompanyId { get; init; }
        public string CompanyName { get; init; } = null!;
        public JobStatus Status { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateTime? SubmittedAtUtc { get; init; }
    }
}
