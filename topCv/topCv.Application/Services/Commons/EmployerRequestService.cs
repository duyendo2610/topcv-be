using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Domain.Common;
using topCv.Domain.Entities.Commons;
using topCv.Domain.Enums;

namespace topCv.Application.Services.Commons
{
    public sealed class EmployerRequestService : IEmployerRequestService
    {
        private readonly IAppDbContext _db;
        private readonly INotificationService _noti;

        public EmployerRequestService(IAppDbContext db, INotificationService noti)
        {
            _db = db;
            _noti = noti;
        }

        public async Task<EmployerRequestResponse> CreateAsync(CreateEmployerRequestRequest req, Guid userId,
            CancellationToken ct)
        {
            var company = await _db.Companies
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.Id == req.CompanyId, ct)
                          ?? throw new KeyNotFoundException("Không tìm thấy công ty.");

            if (company.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Bạn không phải chủ sở hữu công ty.");

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId, ct)
                       ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            if (user.Role == AppRoles.Employer)
                throw new InvalidOperationException("Bạn đã là nhà tuyển dụng.");

            var pendingExists = await _db.EmployerRequests
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.CompanyId == req.CompanyId &&
                    x.Status == EmployerRequestStatus.Pending, ct);

            if (pendingExists)
                throw new InvalidOperationException("Bạn đã gửi yêu cầu trước đó.");

            var entity = new EmployerRequest
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CompanyId = req.CompanyId,
                Message = string.IsNullOrWhiteSpace(req.Message) ? null : req.Message.Trim(),
                Status = EmployerRequestStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.EmployerRequests.Add(entity);
            await _db.SaveChangesAsync(ct);

            await _noti.CreateForRolesAsync(
                new[] { AppRoles.Admin },
                new CreateNotificationTemplateRequest
                {
                    Type = NotificationType.Other,
                    Title = "Yeu cau cap quyen Employer moi",
                    Body = $"{user.FullName} ({user.Email}) vua gui yeu cau cho cong ty '{company.Name}'.",
                },
                ct,
                excludeUserId: userId);

            return new EmployerRequestResponse
            {
                Id = entity.Id,
                UserId = user.Id,
                UserEmail = user.Email,
                UserFullName = user.FullName,
                CompanyId = company.Id,
                CompanyName = company.Name,
                Status = entity.Status,
                Message = entity.Message,
                CreatedAtUtc = entity.CreatedAtUtc
            };
        }

        public async Task<List<EmployerRequestResponse>> GetMineAsync(Guid userId, CancellationToken ct)
        {
            return await _db.EmployerRequests
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new EmployerRequestResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    UserEmail = x.User.Email,
                    UserFullName = x.User.FullName,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.Name,
                    Status = x.Status,
                    Message = x.Message,
                    CreatedAtUtc = x.CreatedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task<List<EmployerRequestResponse>> GetPendingAsync(CancellationToken ct)
        {
            return await _db.EmployerRequests
                .AsNoTracking()
                .Where(x => x.Status == EmployerRequestStatus.Pending)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new EmployerRequestResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    UserEmail = x.User.Email,
                    UserFullName = x.User.FullName,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.Name,
                    Status = x.Status,
                    Message = x.Message,
                    CreatedAtUtc = x.CreatedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task ApproveAsync(Guid requestId, Guid adminUserId, CancellationToken ct)
        {
            var request = await _db.EmployerRequests
                              .FirstOrDefaultAsync(x => x.Id == requestId, ct)
                          ?? throw new KeyNotFoundException("Không tìm thấy yêu cầu.");

            if (request.Status != EmployerRequestStatus.Pending)
                throw new InvalidOperationException("Yêu cầu không còn ở trạng thái chờ duyệt.");

            var user = await _db.Users
                           .FirstOrDefaultAsync(x => x.Id == request.UserId, ct)
                       ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var company = await _db.Companies
                              .FirstOrDefaultAsync(x => x.Id == request.CompanyId, ct)
                          ?? throw new KeyNotFoundException("Không tìm thấy công ty.");

            user.Role = AppRoles.Employer;
            company.OwnerUserId = user.Id;

            request.Status = EmployerRequestStatus.Approved;
            request.ResolvedAtUtc = DateTime.UtcNow;
            request.ResolvedByUserId = adminUserId;

            await _db.SaveChangesAsync(ct);

            await _noti.CreateAsync(new CreateNotificationRequest
            {
                UserId = request.UserId,
                Type = NotificationType.Other,
                Title = "Yeu cau Employer da duoc chap thuan",
                Body = "Tai khoan cua ban da duoc cap quyen Employer.",
            }, ct);
        }

        public async Task RejectAsync(Guid requestId, Guid adminUserId, CancellationToken ct)
        {
            var request = await _db.EmployerRequests
                              .FirstOrDefaultAsync(x => x.Id == requestId, ct)
                          ?? throw new KeyNotFoundException("Không tìm thấy yêu cầu.");

            if (request.Status != EmployerRequestStatus.Pending)
                throw new InvalidOperationException("Yêu cầu không còn ở trạng thái chờ duyệt.");

            request.Status = EmployerRequestStatus.Rejected;
            request.ResolvedAtUtc = DateTime.UtcNow;
            request.ResolvedByUserId = adminUserId;

            await _db.SaveChangesAsync(ct);

            await _noti.CreateAsync(new CreateNotificationRequest
            {
                UserId = request.UserId,
                Type = NotificationType.Other,
                Title = "Yeu cau Employer bi tu choi",
                Body = "Yeu cau cap quyen Employer cua ban da bi tu choi.",
            }, ct);
        }
    }
}
