using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Domain.Entities.Obj
{
    public class JobSkill
    {
        public Guid JobId { get; set; }
        public int SkillId { get; set; }

        public Job Job { get; set; } = default!;
        public Skill Skill { get; set; } = default!;
    }
}
