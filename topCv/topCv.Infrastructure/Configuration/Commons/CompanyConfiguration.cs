using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;

namespace topCv.Infrastructure.Configuration.Commons
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OwnerUserId).IsRequired();
            builder.HasIndex(x => x.OwnerUserId);

            builder.Property(x => x.Name).HasMaxLength(255).IsRequired();

            builder.Property(x => x.TaxCode).HasMaxLength(50);
            builder.Property(x => x.Website).HasMaxLength(255);

            builder.Property(x => x.Description).HasColumnType("text");
            builder.Property(x => x.LogoUrl).HasMaxLength(500);
            builder.Property(x => x.CoverUrl).HasMaxLength(500);

            builder.Property(x => x.Address).HasMaxLength(255);

            builder.Property(x => x.IsVerified).HasDefaultValue(false).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);

            builder.HasOne(x => x.Province)
                .WithMany(x => x.Companies)
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.CityId);
        }
    }
}