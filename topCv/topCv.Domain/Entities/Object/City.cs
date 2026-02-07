using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Domain.Entities.Obj
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public ICollection<CandidateProfile> CandidateProfiles { get; set; } = new List<CandidateProfile>();
        public ICollection<Company> Companies { get; set; } = new List<Company>();
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
