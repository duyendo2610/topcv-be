using topCv.Domain.Entities.Commons;

namespace topCv.Domain.Entities.Auth
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        // Navigation
        public CandidateProfile? CandidateProfile { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
        public ICollection<Company> CompaniesOwned { get; set; } = new List<Company>();
        public ICollection<Job> JobsCreated { get; set; } = new List<Job>();
        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
        public ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
        public ICollection<FollowCompany> FollowCompanies { get; set; } = new List<FollowCompany>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}