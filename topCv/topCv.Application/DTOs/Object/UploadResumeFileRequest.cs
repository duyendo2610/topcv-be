using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.DTOs.Obj
{
    public sealed class UploadResumeFileRequest
    {
        public required Guid ResumeId { get; init; }
    }
}
