using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface IFollowCompanyService
    {
        Task FollowAsync(Guid companyId, Guid userId, CancellationToken ct);
        Task UnfollowAsync(Guid companyId, Guid userId, CancellationToken ct);
        Task<List<FollowCompanyResponse>> GetMyFollowedAsync(Guid userId, CancellationToken ct);
    }
}