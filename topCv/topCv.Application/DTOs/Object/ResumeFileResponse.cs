using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.DTOs.Obj
{

    public sealed class ResumeFileResponse
    {
        public Guid Id { get; init; }
        public Guid ResumeId { get; init; }

        public string FileUrl { get; init; } = null!;
        public string FileName { get; init; } = null!;
        public string MimeType { get; init; } = null!;
        public long FileSize { get; init; }

        public DateTime UploadedAt { get; init; }
    }
}
