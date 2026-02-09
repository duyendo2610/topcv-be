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
