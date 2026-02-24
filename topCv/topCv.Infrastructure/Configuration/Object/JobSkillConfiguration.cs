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
