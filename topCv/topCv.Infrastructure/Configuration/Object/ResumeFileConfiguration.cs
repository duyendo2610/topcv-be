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
    public class ResumeFileConfiguration : IEntityTypeConfiguration<ResumeFile>
    {
        public void Configure(EntityTypeBuilder<ResumeFile> builder)
        {
            builder.ToTable("ResumeFiles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ResumeId).IsRequired();
            builder.HasIndex(x => x.ResumeId);

            builder.Property(x => x.FileUrl).HasMaxLength(800).IsRequired();
            builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
            builder.Property(x => x.FileSize).IsRequired();
            builder.Property(x => x.MimeType).HasMaxLength(100).IsRequired();
            builder.Property(x => x.UploadedAt).IsRequired();
        }
    }
}
