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
    public class CandidateProfileConfiguration : IEntityTypeConfiguration<CandidateProfile>
    {
        public void Configure(EntityTypeBuilder<CandidateProfile> builder)
        {
            builder.ToTable("CandidateProfiles");

            builder.HasKey(x => x.UserId);

            builder.Property(x => x.Title).HasMaxLength(200);
            builder.Property(x => x.Address).HasMaxLength(255);
            builder.Property(x => x.AvatarUrl).HasMaxLength(500);

            builder.Property(x => x.About).HasColumnType("text");

            builder.Property(x => x.YearsOfExperience);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);

            builder.HasOne(x => x.Province)
                 .WithMany(x => x.CandidateProfiles)
                 .HasForeignKey(x => x.CityId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.CityId);
        }
    }
}
