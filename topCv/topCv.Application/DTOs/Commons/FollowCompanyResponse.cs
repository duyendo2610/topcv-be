namespace topCv.Application.DTOs.Commons
{
    public sealed class FollowCompanyResponse
    {
        public Guid CompanyId { get; init; }
        public string CompanyName { get; init; } = null!;
        public string? LogoUrl { get; init; }
        public DateTime FollowedAt { get; init; }
    }
}