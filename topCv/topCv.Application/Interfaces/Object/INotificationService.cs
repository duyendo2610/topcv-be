using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;

namespace topCv.Application.Interfaces.Obj
{

    public interface INotificationService
    {
        Task<List<NotificationResponse>> GetMyAsync(Guid userId, CancellationToken ct);

        Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct);
        Task MarkAllAsReadAsync(Guid userId, CancellationToken ct);

        Task<NotificationResponse> CreateAsync(CreateNotificationRequest req, CancellationToken ct);
    }
}
