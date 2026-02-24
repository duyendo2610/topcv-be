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
