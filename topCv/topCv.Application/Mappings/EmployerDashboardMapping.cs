using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;
using topCv.Domain.Entities.Obj;

namespace topCv.Application.Mappings
{

    public static class EmployerDashboardMapping
    {
        public static EmployerCompanyResponse ToEmployerResponse(this Company entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedAt = entity.CreatedAt
            };

        public static EmployerJobResponse ToEmployerResponse(this Job entity, int totalApplications)
            => new()
            {
                Id = entity.Id,
                Title = entity.Title,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                TotalApplications = totalApplications
            };
    }
}
