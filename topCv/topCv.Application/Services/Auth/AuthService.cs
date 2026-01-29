using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.Common;
using topCv.Application.DTOs.Auth;
using topCv.Application.Interfaces.Auth;
using topCv.Domain.Entities.Auth;

namespace topCv.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAppDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IHashService _hashService;

        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

        public AuthService(
            IAppDbContext db,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IHashService hashService)
        {
            _db = db;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _hashService = hashService;
        }
        public async Task<AuthResponse> LoginAsync(UserLoginRequest request, CancellationToken ct)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email, ct);
            if (user is null || !user.IsActive)
                throw new InvalidOperationException("Sai email hoặc mật khẩu.");

            if (!_passwordHasher.Verify(user, request.Password))
                throw new InvalidOperationException("Sai email hoặc mật khẩu.");

            var accessToken = _tokenService.CreateAccessToken(user, out var accessExpUtc);
            var refreshTokenString = _tokenService.CreateRefreshToken();

            var refreshEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = refreshTokenString,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime),
            };

            _db.RefreshTokens.Add(refreshEntity);
            await _db.SaveChangesAsync(ct);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString
            };
        }

        public async Task<AuthResponse> RegisterAsync(UserRegisterRequest request, CancellationToken ct)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var exists = await _db.Users.AnyAsync(x => x.Email == email, ct);
            if (exists) throw new InvalidOperationException("Email đã tồn tại.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                FullName = request.FullName,
                Role = "User",
                IsActive = true
            };

            user.PasswordHash = _passwordHasher.Hash(user, request.Password);

            _db.Users.Add(user);

            var accessToken = _tokenService.CreateAccessToken(user, out var accessExpUtc);
            var refreshTokenString = _tokenService.CreateRefreshToken();

            var refreshEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = refreshTokenString,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime),
            };

            _db.RefreshTokens.Add(refreshEntity);

            await _db.SaveChangesAsync(ct);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString
            };
        }

        public async Task<AuthResponse> RefreshAsync(UserRefreshTokenRequest request, CancellationToken ct)
        {
            var incoming = request.RefreshToken.Trim();

            var stored = await _db.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == incoming, ct);

            if (stored is null)
                throw new InvalidOperationException("Refresh token không hợp lệ.");

            // Active = chưa revoke + chưa hết hạn
            var isExpired = DateTime.UtcNow >= stored.ExpiresAtUtc;
            var isRevoked = stored.RevokedAtUtc != null;

            if (isExpired || isRevoked)
                throw new InvalidOperationException("Refresh token đã hết hạn hoặc bị thu hồi.");

            // Rotation: thu hồi token cũ
            stored.RevokedAtUtc = DateTime.UtcNow;

            // tạo token mới
            var newRefresh = _tokenService.CreateRefreshToken();
            stored.ReplacedByTokenHash = newRefresh;

            var user = stored.User;
            if (user is null || !user.IsActive)
                throw new InvalidOperationException("User không hợp lệ.");

            var accessToken = _tokenService.CreateAccessToken(user, out var accessExpUtc);

            _db.RefreshTokens.Add(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = newRefresh,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime),
            });

            await _db.SaveChangesAsync(ct);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefresh
            };
        }

        public async Task LogoutAsync(string refreshToken, CancellationToken ct)
        {
            var hash = _hashService.Hash(refreshToken);

            var stored = await _db.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

            if (stored == null)
                return;

            if (stored.RevokedAtUtc != null)
                return;

            stored.RevokedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
        }

        public async Task LogoutAllAsync(Guid userId, CancellationToken ct)
        {
            var tokens = await _db.RefreshTokens
        .Where(x =>
            x.UserId == userId &&
            x.RevokedAtUtc == null &&
            x.ExpiresAtUtc > DateTime.UtcNow)
        .ToListAsync(ct);

            foreach (var token in tokens)
                token.RevokedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
        }
    }
}
