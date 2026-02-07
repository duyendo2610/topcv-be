using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;

namespace topCv.Application.Interfaces.Obj
{
    public interface IJobMatchService
    {
        Task<List<JobMatchResponse>> GetMyMatchesAsync(Guid userId, int take, CancellationToken ct);

        /// Tạo notification JobMatched cho các job phù hợp (không tạo trùng).
        /// Return: số notification đã tạo.
        Task<int> NotifyMyMatchesAsync(Guid userId, int take, CancellationToken ct);
    }
}
