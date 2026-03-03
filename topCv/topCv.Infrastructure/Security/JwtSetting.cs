namespace topCv.Infrastructure.Security
{
    public sealed class JwtSetting
    {
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string Key { get; set; } = default!;
        public int AccessTokenMinutes { get; set; } = 120;
        public int RefreshTokenDays { get; set; } = 7;
    }
}