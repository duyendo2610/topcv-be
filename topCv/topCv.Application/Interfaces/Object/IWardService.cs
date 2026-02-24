using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Object;

namespace topCv.Application.Interfaces.Object
{
    public interface IWardService
    {
        Task<List<WardResponse>> GetAllAsync();
        Task<List<WardResponse>> GetByProvinceIdAsync(int provinceId);
        Task<List<WardResponse>> SearchAsync(string keyword,CancellationToken ct);
    }
}
