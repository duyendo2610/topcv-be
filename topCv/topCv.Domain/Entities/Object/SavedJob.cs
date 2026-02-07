using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Auth;

namespace topCv.Domain.Entities.Obj
{
    public class SavedJob
    {
        public Guid UserId { get; set; }
        public Guid JobId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = default!;
        public Job Job { get; set; } = default!;
    }
}
