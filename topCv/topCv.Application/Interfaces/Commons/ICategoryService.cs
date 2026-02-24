using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface ICategoryService
    {
        Task<List<CategoryResponse>> GetAllAsync(CancellationToken ct);
        Task<CategoryResponse> CreateAsync(CreateCategoryRequest req, CancellationToken ct);
        Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest req, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
        Task<List<CategoryTreeResponse>> GetTreeAsync(CancellationToken ct);
    }
}