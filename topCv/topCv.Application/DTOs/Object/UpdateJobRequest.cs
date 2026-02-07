using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Obj
{
    public sealed class UpdateJobRequest
    {
        public required string Title { get; init; }
        public required JobLevel Level { get; init; }
        public required JobType JobType { get; init; }

        public required string Description { get; init; }
        public string? Requirement { get; init; }
        public string? Benefit { get; init; }

        public decimal? SalaryMin { get; init; }
        public decimal? SalaryMax { get; init; }
        public string? Currency { get; init; }

        public int? CityId { get; init; }
        public string? Address { get; init; }

        public int? ExpMin { get; init; }
        public int? ExpMax { get; init; }

        public DateTime? DeadlineAtUtc { get; init; }

        public List<int> SkillIds { get; init; } = [];
        public List<int> CategoryIds { get; init; } = [];
    }

}
