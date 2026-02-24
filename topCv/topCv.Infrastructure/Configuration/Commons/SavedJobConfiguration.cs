using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;

namespace topCv.Infrastructure.Configuration.Commons
{
    public class SavedJobConfiguration : IEntityTypeConfiguration<SavedJob>
    {
        public void Configure(EntityTypeBuilder<SavedJob> builder)
        {
            builder.ToTable("SavedJobs");

            builder.HasKey(x => new { x.UserId, x.JobId });

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.SavedJobs)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Job)
                .WithMany(x => x.SavedByUsers)
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}