using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface ISavedJobService
    {
        Task SaveAsync(Guid jobId, Guid userId, CancellationToken ct);
        Task UnsaveAsync(Guid jobId, Guid userId, CancellationToken ct);
        Task<List<SavedJobResponse>> GetMySavedJobsAsync(Guid userId, CancellationToken ct);
    }
}