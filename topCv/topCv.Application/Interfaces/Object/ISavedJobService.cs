using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;

namespace topCv.Application.Interfaces.Obj
{
    public interface ISavedJobService
    {
        Task SaveAsync(Guid jobId, Guid userId, CancellationToken ct);
        Task UnsaveAsync(Guid jobId, Guid userId, CancellationToken ct);
        Task<List<SavedJobResponse>> GetMySavedJobsAsync(Guid userId, CancellationToken ct);
    }
}
