using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Auth;

namespace topCv.Application.Interfaces.Auth
{
    internal interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
        Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken ct);
    }
}
