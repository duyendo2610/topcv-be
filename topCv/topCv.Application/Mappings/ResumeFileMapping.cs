using topCv.Application.DTOs.Commons;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Mappings
{
    public static class ResumeFileMapping
    {
        public static ResumeFileResponse ToResponse(this ResumeFile entity)
            => new()
            {
                Id = entity.Id,
                ResumeId = entity.ResumeId,
                FileUrl = entity.FileUrl,
                FileName = entity.FileName,
                MimeType = entity.MimeType,
                FileSize = entity.FileSize,
                UploadedAt = entity.UploadedAt
            };
    }
}