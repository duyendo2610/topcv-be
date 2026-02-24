using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Mappings;

namespace topCv.Application.Services.Commons
{
    public sealed class NotificationService : INotificationService
    {
        private readonly IAppDbContext _db;

        public NotificationService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<NotificationResponse> CreateAsync(CreateNotificationRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                throw new ArgumentException("Title is required.");

            var entity = req.ToEntity();

            _db.Notifications.Add(entity);
            await _db.SaveChangesAsync(ct);

            return entity.ToResponse();
        }

        public async Task<List<NotificationResponse>> GetMyAsync(Guid userId, CancellationToken ct)
        {
            var items = await _db.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);

            return items.Select(x => x.ToResponse()).ToList();
        }

        public async Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct)
        {
            var entity = await _db.Notifications
                             .FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId, ct)
                         ?? throw new KeyNotFoundException("Notification not found.");

            if (!entity.IsRead)
            {
                entity.IsRead = true;
                await _db.SaveChangesAsync(ct);
            }
        }

        public async Task MarkAllAsReadAsync(Guid userId, CancellationToken ct)
        {
            var unread = await _db.Notifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .ToListAsync(ct);

            if (unread.Count == 0) return;

            foreach (var n in unread)
                n.IsRead = true;

            await _db.SaveChangesAsync(ct);
        }
    }
}