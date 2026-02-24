using topCv.Application.DTOs.Auth;

namespace topCv.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(UserRegisterRequest request, CancellationToken ct);
        Task<AuthResponse> LoginAsync(UserLoginRequest request, CancellationToken ct);
        Task<AuthResponse> RefreshAsync(UserRefreshTokenRequest request, CancellationToken ct);
        Task LogoutAsync(string refreshToken, CancellationToken ct);
        Task LogoutAllAsync(Guid userId, CancellationToken ct);
    }
}