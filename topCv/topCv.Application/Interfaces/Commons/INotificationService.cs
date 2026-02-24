using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface INotificationService
    {
        Task<List<NotificationResponse>> GetMyAsync(Guid userId, CancellationToken ct);
        Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct);
        Task MarkAllAsReadAsync(Guid userId, CancellationToken ct);
        Task<NotificationResponse> CreateAsync(CreateNotificationRequest req, CancellationToken ct);
    }
}