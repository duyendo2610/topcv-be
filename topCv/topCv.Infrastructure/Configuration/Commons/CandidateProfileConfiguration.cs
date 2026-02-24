using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;

namespace topCv.Infrastructure.Configuration.Commons
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