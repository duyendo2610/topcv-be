using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using topCv.Domain.Entities.Auth;
using topCv.Domain.Entities.Obj;
using topCv.Domain.Entities.Object;

namespace topCv.Application.Common
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<Province> Provinces { get;}
        DbSet<Ward> Wards { get;}

        DbSet<CandidateProfile> CandidateProfiles { get; }
        DbSet<Resume> Resumes { get; }
        DbSet<ResumeSection> ResumeSections { get; }
        DbSet<ResumeFile> ResumeFiles { get;}

        DbSet<Company> Companies { get; }
        DbSet<Job> Jobs { get;}

        DbSet<Skill> Skills { get; }
        DbSet<JobSkill> JobSkills { get;}

        DbSet<Category> Categories { get; }
        DbSet<JobCategory> JobCategories { get;}

        DbSet<JobApplication> JobApplications { get; }
        DbSet<SavedJob> SavedJobs { get; }
        DbSet<FollowCompany> FollowCompanies { get;}

        DbSet<Notification> Notifications { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
