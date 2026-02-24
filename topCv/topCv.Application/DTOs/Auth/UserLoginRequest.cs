namespace topCv.Application.DTOs.Auth
{
    public class UserLoginRequest
    {
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
    }
}