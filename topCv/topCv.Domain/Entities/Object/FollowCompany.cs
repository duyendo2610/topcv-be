using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Auth;

namespace topCv.Domain.Entities.Obj
{
    public class FollowCompany
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = default!;
        public Company Company { get; set; } = default!;
    }
}
