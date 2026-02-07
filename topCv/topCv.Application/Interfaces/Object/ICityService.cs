using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;

namespace topCv.Application.Interfaces.Obj
{
    public interface ICityService
    {
        Task<List<IdNameResponse>> GetAllAsync(CancellationToken ct);
        Task<IdNameResponse> CreateAsync(CreateNameRequest req, CancellationToken ct);
        Task<IdNameResponse> UpdateAsync(int id, UpdateNameRequest req, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }
}
