using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.DTOs.Obj
{
    public sealed class FollowCompanyResponse
    {
        public Guid CompanyId { get; init; }
        public string CompanyName { get; init; } = null!;
        public string? LogoUrl { get; init; }
        public DateTime FollowedAt { get; init; }
    }
}
