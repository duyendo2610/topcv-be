using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using topCv.Application.Interfaces.Auth;
using topCv.Domain.Entities.Auth;

namespace topCv.Infrastructure.Security
{
    public class TokenService : ITokenService
    {
        private readonly JwtSetting _jwt;

        public TokenService(IOptions<JwtSetting> jwt)
        {
            _jwt = jwt.Value;
        }

        public string CreateAccessToken(User user, out DateTime expiresAtUtc)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes);

            var claims = new[]
            {
                // Include both JWT standard claims and .NET ClaimTypes for compatibility.
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }
    }
}
