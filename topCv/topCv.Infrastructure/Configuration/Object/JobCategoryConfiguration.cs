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
    public class JobCategoryConfiguration : IEntityTypeConfiguration<JobCategory>
    {
        public void Configure(EntityTypeBuilder<JobCategory> builder)
        {
            builder.ToTable("JobCategories");

            builder.HasKey(x => new { x.JobId, x.CategoryId });

            builder.HasOne(x => x.Job)
                 .WithMany(x => x.JobCategories)
                 .HasForeignKey(x => x.JobId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Category)
                 .WithMany(x => x.JobCategories)
                 .HasForeignKey(x => x.CategoryId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
