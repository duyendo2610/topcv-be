using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class JobQueryRequest
    {
        public string? Keyword { get; init; }
        public int? CityId { get; init; }
        public JobLevel? Level { get; init; }
        public JobType? JobType { get; init; }
        public List<int>? SkillIds { get; init; }
        public List<int>? CategoryIds { get; init; }
        public decimal? SalaryMin { get; init; }
        public decimal? SalaryMax { get; init; }
        public string? Currency { get; init; }
        public int? ExpMin { get; init; }
        public int? ExpMax { get; init; }
        public string? SortBy { get; init; }
        public string? SortDirection { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
