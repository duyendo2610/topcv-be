using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.Common;
using topCv.Domain.Entities.Auth;
using topCv.Domain.Entities.Obj;
using topCv.Infrastructure.Configuration.Auth;

namespace topCv.Infrastructure.Persistence
{
    public sealed class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<City> Cities { get; set; }

        public DbSet<CandidateProfile> CandidateProfiles { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<ResumeSection> ResumeSections { get; set; }
        public DbSet<ResumeFile> ResumeFiles { get; set; }

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
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        }
    }
}
