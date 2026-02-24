using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class EmployerJobResponse
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = null!;
        public JobStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
        public int TotalApplications { get; init; }
    }
}