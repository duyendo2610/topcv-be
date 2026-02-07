using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Domain.Entities.Obj
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }
}
