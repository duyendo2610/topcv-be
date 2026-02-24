using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface IProvinceService
    {
        Task<List<ProvinceResponse>> GetAllAsync(CancellationToken ct);
        Task<List<ProvinceResponse>> SearchAsync(string keyword, CancellationToken ct);
    }
}