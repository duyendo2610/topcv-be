using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class EmployerRequestResponse
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string UserEmail { get; init; } = null!;
        public string UserFullName { get; init; } = null!;
        public Guid CompanyId { get; init; }
        public string CompanyName { get; init; } = null!;
        public EmployerRequestStatus Status { get; init; }
        public string? Message { get; init; }
        public DateTime CreatedAtUtc { get; init; }
    }
}
