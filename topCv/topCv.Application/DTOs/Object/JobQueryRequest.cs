using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Obj
{
    public sealed class JobQueryRequest
    {
        public string? Keyword { get; init; }
        public int? CityId { get; init; }
        public JobLevel? Level { get; init; }
        public JobType? JobType { get; init; }

        public List<int>? SkillIds { get; init; }
        public List<int>? CategoryIds { get; init; }

        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }

}
