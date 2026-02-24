namespace topCv.Application.DTOs.Commons
{
    public sealed class JobMatchResponse
    {
        public Guid JobId { get; init; }
        public string Title { get; init; } = null!;
        public Guid CompanyId { get; init; }
        public string CompanyName { get; init; } = null!;
        public int? CityId { get; init; }
        public string? CityName { get; init; }
        public decimal? SalaryMin { get; init; }
        public decimal? SalaryMax { get; init; }
        public DateTime CreatedAt { get; init; }
        public int Score { get; init; }
    }
}