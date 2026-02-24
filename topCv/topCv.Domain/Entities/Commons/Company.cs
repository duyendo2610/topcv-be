using topCv.Domain.Entities.Auth;
using topCv.Domain.Enums;

namespace topCv.Domain.Entities.Commons
{
    public class Company
    {
        public Guid Id { get; set; }
        public Guid OwnerUserId { get; set; } // recruiter/owner
        public string Name { get; set; } = default!;
        public string? TaxCode { get; set; }
        public string? Website { get; set; }
        public CompanySize? Size { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? CoverUrl { get; set; }
        public int? CityId { get; set; }
        public string? Address { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public User OwnerUser { get; set; } = default!;
        public Province? Province { get; set; }
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
        public ICollection<FollowCompany> Followers { get; set; } = new List<FollowCompany>();
    }
}