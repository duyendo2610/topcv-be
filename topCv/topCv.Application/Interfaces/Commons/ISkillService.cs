using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface ISkillService
    {
        Task<List<IdNameResponse>> GetAllAsync(CancellationToken ct);
        Task<IdNameResponse> CreateAsync(CreateNameRequest req, CancellationToken ct);
        Task<IdNameResponse> UpdateAsync(int id, UpdateNameRequest req, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }
}