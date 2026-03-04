using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Domain.Entities.Auth;
using topCv.Domain.Entities.Commons;
using topCv.Infrastructure.Configuration.Auth;
using topCv.Infrastructure.Configuration.Commons;

namespace topCv.Infrastructure.Persistence
{
    public sealed class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Auth
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // Commons
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<CandidateProfile> CandidateProfiles { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<ResumeSection> ResumeSections { get; set; }
        public DbSet<ResumeFile> ResumeFiles { get; set; }
        public DbSet<ResumeTemplateVariant> ResumeTemplateVariants { get; set; }
        public DbSet<ResumeThemePreset> ResumeThemePresets { get; set; }
        public DbSet<ResumeTemplateVariantTheme> ResumeTemplateVariantThemes { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<SavedJob> SavedJobs { get; set; }
        public DbSet<FollowCompany> FollowCompanies { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
            => base.SaveChangesAsync(ct);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Auth
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

            // Commons
            modelBuilder.ApplyConfiguration(new CandidateProfileConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new FollowCompanyConfiguration());
            modelBuilder.ApplyConfiguration(new JobApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new JobCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new JobConfiguration());
            modelBuilder.ApplyConfiguration(new JobSkillConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new ProvinceConfiguration());
            modelBuilder.ApplyConfiguration(new ResumeConfiguration());
            modelBuilder.ApplyConfiguration(new ResumeFileConfiguration());
            modelBuilder.ApplyConfiguration(new ResumeSectionConfiguration());
            modelBuilder.ApplyConfiguration(new ResumeTemplateVariantConfiguration());
            modelBuilder.ApplyConfiguration(new ResumeThemePresetConfiguration());
            modelBuilder.ApplyConfiguration(new ResumeTemplateVariantThemeConfiguration());
            modelBuilder.ApplyConfiguration(new SavedJobConfiguration());
            modelBuilder.ApplyConfiguration(new SkillConfiguration());
        }
    }
}
