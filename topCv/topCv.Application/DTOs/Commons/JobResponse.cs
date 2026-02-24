using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class JobResponse
    {
        public Guid Id { get; init; }
        public Guid CompanyId { get; init; }
        public string CompanyName { get; init; } = null!;
        public string? CompanyLogoUrl { get; init; }
        public Guid CreatedByUserId { get; init; }
        public string Title { get; init; } = null!;
        public JobLevel Level { get; init; }
        public JobType JobType { get; init; }
        public string Description { get; init; } = null!;
        public string? Requirement { get; init; }
        public string? Benefit { get; init; }
        public decimal? SalaryMin { get; init; }
        public decimal? SalaryMax { get; init; }
        public string? Currency { get; init; }
        public int? CityId { get; init; }
        public string? CityName { get; init; }
        public string? Address { get; init; }
        public int? ExpMin { get; init; }
        public int? ExpMax { get; init; }
        public DateTime? DeadlineAtUtc { get; init; }
        public JobStatus Status { get; init; }
        public List<int> SkillIds { get; init; } = [];
        public List<int> CategoryIds { get; init; } = [];
    }
}