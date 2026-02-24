namespace topCv.Application.DTOs.Auth
{
    public class UserRefreshTokenRequest
    {
        public string RefreshToken { get; init; } = default!;
    }
}