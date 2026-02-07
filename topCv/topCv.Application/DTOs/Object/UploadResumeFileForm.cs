using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace topCv.Application.DTOs.Obj
{
    public sealed class UploadResumeFileForm
    {
        public required Guid ResumeId { get; init; }
        public required IFormFile File { get; init; }
    }
}
