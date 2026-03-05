using topCv.Domain.Entities.Auth;
using topCv.Domain.Enums;

namespace topCv.Domain.Entities.Commons
{
    public class EmployerRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public string? Message { get; set; }
        public EmployerRequestStatus Status { get; set; } = EmployerRequestStatus.Pending;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ResolvedAtUtc { get; set; }
        public Guid? ResolvedByUserId { get; set; }

        public User User { get; set; } = default!;
        public Company Company { get; set; } = default!;
    }
}
