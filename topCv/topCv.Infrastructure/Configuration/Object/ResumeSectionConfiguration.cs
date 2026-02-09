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
    public class ResumeSectionConfiguration : IEntityTypeConfiguration<ResumeSection>
    {
        public void Configure(EntityTypeBuilder<ResumeSection> builder)
        {
            builder.ToTable("ResumeSections");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ResumeId).IsRequired();
            builder.HasIndex(x => x.ResumeId);

            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(200);

            builder.Property(x => x.SortOrder).HasDefaultValue(0).IsRequired();

            builder.Property(x => x.ContentJson).HasColumnType("text").IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}
