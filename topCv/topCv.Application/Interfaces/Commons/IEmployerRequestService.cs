using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface IEmployerRequestService
    {
        Task<EmployerRequestResponse> CreateAsync(CreateEmployerRequestRequest req, Guid userId, CancellationToken ct);
        Task<List<EmployerRequestResponse>> GetMineAsync(Guid userId, CancellationToken ct);
        Task<List<EmployerRequestResponse>> GetPendingAsync(CancellationToken ct);
        Task ApproveAsync(Guid requestId, Guid adminUserId, CancellationToken ct);
        Task RejectAsync(Guid requestId, Guid adminUserId, CancellationToken ct);
    }
}
