using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;

namespace topCv.Infrastructure.Configuration.Commons
{
    public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
    {
        public void Configure(EntityTypeBuilder<Province> buider)
        {
            buider.ToTable("Provinces");

            buider.HasKey(x => x.Id);

            buider.Property(x => x.Name).HasMaxLength(120).IsRequired();
            buider.HasIndex(x => x.Code).IsUnique();

            buider.HasMany(p => p.Wards)
                .WithOne(w => w.Province)
                .HasForeignKey(w => w.ProvinceId);
        }
    }
}