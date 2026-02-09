using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;
using topCv.Application.DTOs.Object;

namespace topCv.Application.Interfaces.Obj
{
    public interface IProvinceService
    {
        Task<List<ProvinceResponse>> GetAllAsync(CancellationToken ct);
        Task<List<ProvinceResponse>> SearchAsync(string keyword, CancellationToken ct);
    }
}
