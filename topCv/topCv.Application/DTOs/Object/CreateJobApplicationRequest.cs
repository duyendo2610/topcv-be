using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.DTOs.Obj
{
    public sealed class CreateJobApplicationRequest
    {
        public required Guid JobId { get; init; }

        public required Guid ResumeId { get; init; }

        public Guid? ResumeFileId { get; init; }

        public string? CoverLetter { get; init; }
    }
}
