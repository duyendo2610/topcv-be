using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Auth;
using topCv.Domain.Enums;

namespace topCv.Domain.Entities.Obj
{
    public class Notification
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }

        public string Title { get; set; } = default!;
        public string? Body { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = default!;
    }
}
