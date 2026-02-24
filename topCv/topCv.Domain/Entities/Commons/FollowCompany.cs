using topCv.Domain.Entities.Auth;

namespace topCv.Domain.Entities.Commons
{
    public class FollowCompany
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; } = default!;
        public Company Company { get; set; } = default!;
    }
}