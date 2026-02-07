using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Domain.Entities.Obj
{
    public class JobCategory
    {
        public Guid JobId { get; set; }
        public int CategoryId { get; set; }

        public Job Job { get; set; } = default!;
        public Category Category { get; set; } = default!;
    }
}
