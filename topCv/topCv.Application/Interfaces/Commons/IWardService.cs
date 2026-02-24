using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface IWardService
    {
        Task<List<WardResponse>> GetAllAsync();
        Task<List<WardResponse>> GetByProvinceIdAsync(int provinceId);
        Task<List<WardResponse>> SearchAsync(string keyword, CancellationToken ct);
    }
}