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
                throw new ArgumentException("Tiêu đề không được để trống.");

            var entity = req.ToEntity();

            _db.Notifications.Add(entity);
            await _db.SaveChangesAsync(ct);

            return entity.ToResponse();
        }

        public async Task<int> CreateForRolesAsync(
            IEnumerable<string> roles,
            CreateNotificationTemplateRequest req,
            CancellationToken ct,
            Guid? excludeUserId = null)
        {
            var normalizedRoles = roles
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (normalizedRoles.Count == 0 || string.IsNullOrWhiteSpace(req.Title))
                return 0;

            var usersQuery = _db.Users
                .AsNoTracking()
                .Where(x => normalizedRoles.Contains(x.Role));

            if (excludeUserId is Guid userId)
                usersQuery = usersQuery.Where(x => x.Id != userId);

            var userIds = await usersQuery
                .Select(x => x.Id)
                .ToListAsync(ct);

            if (userIds.Count == 0)
                return 0;

            var entities = userIds.Select(userId => new CreateNotificationRequest
            {
                UserId = userId,
                Type = req.Type,
                Title = req.Title,
                Body = req.Body,
            }.ToEntity()).ToList();

            _db.Notifications.AddRange(entities);
            await _db.SaveChangesAsync(ct);
            return entities.Count;
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
                         ?? throw new KeyNotFoundException("Không tìm thấy thông báo.");

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
