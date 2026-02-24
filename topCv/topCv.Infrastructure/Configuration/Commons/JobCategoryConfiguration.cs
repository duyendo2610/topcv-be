using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;

namespace topCv.Infrastructure.Configuration.Commons
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