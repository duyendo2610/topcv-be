namespace topCv.Application.DTOs.Auth
{
    public class LogoutRequest
    {
        public string RefreshToken { get; init; } = null!;
    }
}