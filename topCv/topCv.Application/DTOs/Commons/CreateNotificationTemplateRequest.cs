using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class CreateNotificationTemplateRequest
    {
        public required NotificationType Type { get; init; }
        public required string Title { get; init; }
        public string? Body { get; init; }
    }
}
