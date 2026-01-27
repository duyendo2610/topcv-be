using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Auth;

namespace topCv.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(UserRegisterRequest request, CancellationToken ct);
        Task<AuthResponse> LoginAsync(UserLoginRequest request, CancellationToken ct);
        Task<AuthResponse> RefreshAsync(UserRefreshTokenRequest request, CancellationToken ct);
    }
}
