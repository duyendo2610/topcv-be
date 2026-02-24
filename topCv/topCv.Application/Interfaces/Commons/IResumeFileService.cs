using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface IResumeFileService
    {
        Task<ResumeFileResponse> UploadAsync(Guid userId, Guid resumeId, Stream content, string fileName,
            string contentType, long fileSize, CancellationToken ct);

        Task<List<ResumeFileResponse>> GetByResumeAsync(Guid userId, Guid resumeId, CancellationToken ct);
        Task<ResumeFileResponse> GetByIdAsync(Guid userId, Guid id, CancellationToken ct);
        Task DeleteAsync(Guid userId, Guid id, CancellationToken ct);
    }
}