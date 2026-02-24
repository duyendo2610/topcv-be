namespace topCv.Application.DTOs.Auth
{
    public class UserRegisterRequest
    {
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string FullName { get; init; } = default!;
    }
}