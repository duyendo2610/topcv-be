using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;

namespace topCv.Infrastructure.Configuration.Commons
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("Jobs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompanyId).IsRequired();
            builder.HasIndex(x => x.CompanyId);

            builder.Property(x => x.CreatedByUserId).IsRequired();
            builder.HasIndex(x => x.CreatedByUserId);

            builder.Property(x => x.Title).HasMaxLength(255).IsRequired();
            builder.Property(x => x.Level).IsRequired();
            builder.Property(x => x.JobType).IsRequired();

            builder.Property(x => x.Description).HasColumnType("text").IsRequired();
            builder.Property(x => x.Requirement).HasColumnType("text");
            builder.Property(x => x.Benefit).HasColumnType("text");

            builder.Property(x => x.SalaryMin).HasColumnType("decimal(18,2)");
            builder.Property(x => x.SalaryMax).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Currency).HasMaxLength(10);

            builder.Property(x => x.Address).HasMaxLength(255);

            builder.Property(x => x.Status).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);

            builder.HasOne(x => x.Company)
                .WithMany(x => x.Jobs)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Province)
                .WithMany(x => x.Jobs)
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.CityId);
        }
    }
}