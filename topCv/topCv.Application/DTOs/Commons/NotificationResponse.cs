using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
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