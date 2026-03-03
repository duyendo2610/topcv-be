using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface IResumeService
    {
        Task<ResumeDetailResponse> CreateAsync(Guid userId, CreateResumeRequest req, CancellationToken ct);
        Task<List<ResumeSummaryResponse>> GetMineAsync(Guid userId, CancellationToken ct);
        Task<ResumeDetailResponse> GetByIdAsync(Guid userId, Guid resumeId, CancellationToken ct);
        Task<ResumeDetailResponse> UpdateAsync(Guid userId, Guid resumeId, UpdateResumeRequest req, CancellationToken ct);
        Task DeleteAsync(Guid userId, Guid resumeId, CancellationToken ct);
        Task<ResumePreviewResponse> PreviewAsync(Guid userId, Guid resumeId, CancellationToken ct);
        Task<ResumeFileResponse> ExportHtmlAsync(Guid userId, Guid resumeId, CancellationToken ct);
    }
}
