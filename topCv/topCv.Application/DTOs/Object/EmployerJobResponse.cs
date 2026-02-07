using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Obj
{
    public sealed class EmployerJobResponse
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = null!;
        public JobStatus Status { get; init; }

        public DateTime CreatedAt { get; init; }
        public int TotalApplications { get; init; }
    }
}
