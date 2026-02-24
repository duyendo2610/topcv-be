using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Obj;

namespace topCv.Infrastructure.Configuration.Obj
{
    public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.ToTable("JobApplications");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.JobId).IsRequired();
            builder.HasIndex(x => x.JobId);

            builder.Property(x => x.CandidateUserId).IsRequired();
            builder.HasIndex(x => x.CandidateUserId);

            builder.Property(x => x.CoverLetter).HasColumnType("text");
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.AppliedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);

            builder.HasOne(x => x.Job)
                 .WithMany(x => x.JobApplications)
                 .HasForeignKey(x => x.JobId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Resume)
                 .WithMany(x => x.JobApplications)
                 .HasForeignKey(x => x.ResumeId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ResumeFile)
                 .WithMany(x => x.JobApplications)
                 .HasForeignKey(x => x.ResumeFileId)
                 .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.CandidateUser)
                .WithMany(c => c.JobApplications)
                .HasForeignKey(x => x.CandidateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.JobId, x.CandidateUserId }).IsUnique();
        }
    }
}
