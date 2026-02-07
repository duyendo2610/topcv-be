using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;
using topCv.Domain.Entities.Obj;

namespace topCv.Application.Mappings
{
    public static class SavedJobMapping
    {
        public static SavedJobResponse ToResponse(this SavedJob entity)
            => new()
            {
                JobId = entity.JobId,
                JobTitle = entity.Job?.Title ?? string.Empty,
                CompanyId = entity.Job?.CompanyId ?? Guid.Empty,
                CompanyName = entity.Job?.Company?.Name ?? string.Empty,
                SavedAt = entity.CreatedAt
            };
    }
}
