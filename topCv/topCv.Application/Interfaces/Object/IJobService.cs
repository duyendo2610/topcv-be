using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;

namespace topCv.Application.Interfaces.Obj
{
    public interface IJobService
    {
        Task<JobResponse> CreateAsync(CreateJobRequest req, Guid userId, CancellationToken ct);
        Task<JobResponse> UpdateAsync(Guid id, UpdateJobRequest req, Guid userId, CancellationToken ct);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken ct);

        Task<JobResponse> GetByIdAsync(Guid id, CancellationToken ct);
        Task<List<JobResponse>> SearchAsync(JobQueryRequest req, CancellationToken ct);

        Task PublishAsync(Guid id, Guid userId, CancellationToken ct);
        Task CloseAsync(Guid id, Guid userId, CancellationToken ct);
    }
}
