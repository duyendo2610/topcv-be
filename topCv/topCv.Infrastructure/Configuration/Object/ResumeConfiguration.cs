using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Obj;
using topCv.Domain.Enums;

namespace topCv.Infrastructure.Configuration.Obj
{
    public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
    {
        public void Configure(EntityTypeBuilder<Resume> builder)
        {
            builder.ToTable("Resumes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();

            builder.Property(x => x.Visibility)
                   .HasDefaultValue((ResumeVisibility)0);
            builder.Property(x => x.IsDefault).HasDefaultValue(false).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);

            // Child: sections/files có thể cascade theo schema note
            builder.HasMany(x => x.Sections)
                 .WithOne(x => x.Resume)
                 .HasForeignKey(x => x.ResumeId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Files)
                 .WithOne(x => x.Resume)
                 .HasForeignKey(x => x.ResumeId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
