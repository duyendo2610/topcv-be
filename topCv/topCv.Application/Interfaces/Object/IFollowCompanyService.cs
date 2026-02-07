using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;

namespace topCv.Application.Interfaces.Obj
{
    public interface IFollowCompanyService
    {
        Task FollowAsync(Guid companyId, Guid userId, CancellationToken ct);
        Task UnfollowAsync(Guid companyId, Guid userId, CancellationToken ct);
        Task<List<FollowCompanyResponse>> GetMyFollowedAsync(Guid userId, CancellationToken ct);
    }
}
