using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.DTOs.Obj
{
    public sealed class SavedJobResponse
    {
        public Guid JobId { get; init; }
        public string JobTitle { get; init; } = null!;

        public Guid CompanyId { get; init; }
        public string CompanyName { get; init; } = null!;

        public DateTime SavedAt { get; init; }
    }
}
