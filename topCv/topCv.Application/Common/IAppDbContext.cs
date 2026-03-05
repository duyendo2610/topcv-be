using Microsoft.EntityFrameworkCore;
using topCv.Domain.Entities.Auth;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Common
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<Province> Provinces { get; }
        DbSet<Ward> Wards { get; }

        DbSet<CandidateProfile> CandidateProfiles { get; }
        DbSet<Resume> Resumes { get; }
        DbSet<ResumeSection> ResumeSections { get; }
        DbSet<ResumeFile> ResumeFiles { get; }
        DbSet<ResumeTemplateVariant> ResumeTemplateVariants { get; }
        DbSet<ResumeThemePreset> ResumeThemePresets { get; }
        DbSet<ResumeTemplateVariantTheme> ResumeTemplateVariantThemes { get; }

        DbSet<Company> Companies { get; }
        DbSet<Job> Jobs { get; }

        DbSet<Skill> Skills { get; }
        DbSet<JobSkill> JobSkills { get; }

        DbSet<Category> Categories { get; }
        DbSet<JobCategory> JobCategories { get; }

        DbSet<JobApplication> JobApplications { get; }
        DbSet<SavedJob> SavedJobs { get; }
        DbSet<FollowCompany> FollowCompanies { get; }

        DbSet<Notification> Notifications { get; }
        DbSet<EmployerRequest> EmployerRequests { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
