using topCv.Application.DTOs.Commons;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Mappings
{
    public static class NotificationMapping
    {
        public static NotificationResponse ToResponse(this Notification entity)
            => new()
            {
                Id = entity.Id,
                Type = entity.Type,
                Title = entity.Title,
                Body = entity.Body,
                IsRead = entity.IsRead,
                CreatedAt = entity.CreatedAt
            };

        public static Notification ToEntity(this CreateNotificationRequest req)
            => new()
            {
                Id = Guid.NewGuid(),
                UserId = req.UserId,
                Type = req.Type,
                Title = req.Title.Trim(),
                Body = req.Body,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
    }
}