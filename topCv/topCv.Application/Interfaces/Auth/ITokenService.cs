using topCv.Domain.Entities.Auth;

namespace topCv.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        string CreateAccessToken(User user, out DateTime expiresAtUtc);
        string CreateRefreshToken();
    }
}