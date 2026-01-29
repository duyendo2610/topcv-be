using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;
using topCv.Domain.Entities.Obj;

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
