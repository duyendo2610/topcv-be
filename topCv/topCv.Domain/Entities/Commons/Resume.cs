using topCv.Domain.Entities.Auth;
using topCv.Domain.Enums;

namespace topCv.Domain.Entities.Commons
{
    public class Resume
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = default!;
        public ResumeVisibility Visibility { get; set; } = ResumeVisibility.Private;
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public User User { get; set; } = default!;
        public ICollection<ResumeSection> Sections { get; set; } = new List<ResumeSection>();
        public ICollection<ResumeFile> Files { get; set; } = new List<ResumeFile>();
        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    }
}