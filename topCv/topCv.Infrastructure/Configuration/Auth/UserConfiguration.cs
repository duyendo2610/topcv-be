using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Auth;
using topCv.Domain.Entities.Obj;

namespace topCv.Infrastructure.Configuration.Auth
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email).HasMaxLength(255).IsRequired();
            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.PasswordHash).IsRequired();

            builder.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Phone).HasMaxLength(30);

            builder.Property(x => x.Role).IsRequired(); 
            builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();

            builder.Property(x => x.CreatedAtUtc).IsRequired();

            builder.HasMany(x => x.RefreshTokens)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CandidateProfile)
                .WithOne(x => x.User)
                .HasForeignKey<CandidateProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Resumes)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.CompaniesOwned)
                .WithOne(x => x.OwnerUser)
                .HasForeignKey(x => x.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.JobsCreated)
                .WithOne(x => x.CreatedByUser)
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.JobApplications)
                .WithOne(x => x.CandidateUser)
                .HasForeignKey(x => x.CandidateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Notifications)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
