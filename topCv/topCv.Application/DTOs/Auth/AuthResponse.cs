namespace topCv.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
    }
}