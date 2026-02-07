using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.DTOs.Obj
{
    public sealed class UpdateJobApplicationRequest
    {

        public Guid? ResumeId { get; init; }

        public Guid? ResumeFileId { get; init; }

        public string? CoverLetter { get; init; }
    }
}
