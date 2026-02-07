using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;

namespace topCv.Application.Interfaces.Obj
{
    public interface IResumeFileService
    {
        Task<ResumeFileResponse> UploadAsync(Guid userId,Guid resumeId,Stream content,string fileName,string contentType,long fileSize,CancellationToken ct);

        Task<List<ResumeFileResponse>> GetByResumeAsync(Guid userId, Guid resumeId, CancellationToken ct);

        Task<ResumeFileResponse> GetByIdAsync(Guid userId, Guid id, CancellationToken ct);

        Task DeleteAsync(Guid userId, Guid id, CancellationToken ct);
    }
}
