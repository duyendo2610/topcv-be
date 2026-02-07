using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Obj
{
    public sealed class CreateNotificationRequest
    {
        public required Guid UserId { get; init; }
        public required NotificationType Type { get; init; }
        public required string Title { get; init; }
        public string? Body { get; init; }
    }
}
