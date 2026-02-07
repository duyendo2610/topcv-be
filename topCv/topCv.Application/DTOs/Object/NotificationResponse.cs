using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Obj
{
    public sealed class NotificationResponse
    {
        public Guid Id { get; init; }

        public NotificationType Type { get; init; }

        public string Title { get; init; } = null!;
        public string? Body { get; init; }

        public bool IsRead { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
