using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;

namespace topCv.Infrastructure.Configuration.Commons
{
    public class JobSkillConfiguration : IEntityTypeConfiguration<JobSkill>
    {
        public void Configure(EntityTypeBuilder<JobSkill> builder)
        {
            builder.ToTable("JobSkills");

            builder.HasKey(x => new { x.JobId, x.SkillId });

            builder.HasOne(x => x.Job)
                .WithMany(x => x.JobSkills)
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Skill)
                .WithMany(x => x.JobSkills)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}