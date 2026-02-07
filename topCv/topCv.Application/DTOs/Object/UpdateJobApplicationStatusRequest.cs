using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Obj
{
    public sealed class UpdateJobApplicationStatusRequest
    {
        public required ApplicationStatus Status { get; init; } 
        public string? Note { get; init; }
    }
}
