using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface IJobService
    {
        Task<JobResponse> CreateAsync(CreateJobRequest req, Guid userId, CancellationToken ct);
        Task<JobResponse> UpdateAsync(Guid id, UpdateJobRequest req, Guid userId, CancellationToken ct);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken ct);
        Task<JobResponse> GetByIdAsync(Guid id, CancellationToken ct);
        Task<PagedResult<JobResponse>> SearchAsync(JobQueryRequest req, CancellationToken ct);
        Task PublishAsync(Guid id, Guid userId, CancellationToken ct);
        Task CloseAsync(Guid id, Guid userId, CancellationToken ct);
        Task<List<AdminJobApprovalResponse>> GetPendingApprovalsAsync(CancellationToken ct);
        Task ApproveAsync(Guid id, Guid adminUserId, CancellationToken ct);
        Task RejectAsync(Guid id, Guid adminUserId, CancellationToken ct);
    }
}
