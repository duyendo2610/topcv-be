using Microsoft.AspNetCore.Http;

namespace topCv.Application.DTOs.Commons
{
    public sealed class UploadResumeFileForm
    {
        public required Guid ResumeId { get; init; }
        public required IFormFile File { get; init; }
    }
}