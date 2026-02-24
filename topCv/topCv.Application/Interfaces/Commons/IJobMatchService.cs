using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface IJobMatchService
    {
        Task<List<JobMatchResponse>> GetMyMatchesAsync(Guid userId, int take, CancellationToken ct);

        /// Tạo notification JobMatched cho các job phù hợp (không tạo trùng).
        /// Return: số notification đã tạo.
        Task<int> NotifyMyMatchesAsync(Guid userId, int take, CancellationToken ct);
    }
}