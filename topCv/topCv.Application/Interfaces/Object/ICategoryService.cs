using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;

namespace topCv.Application.Interfaces.Obj
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
