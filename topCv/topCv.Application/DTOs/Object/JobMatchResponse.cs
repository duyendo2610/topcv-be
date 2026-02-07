using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.DTOs.Obj
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

        // điểm gợi ý để debug/mở rộng ranking
        public int Score { get; init; }
    }
}
