using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;
using topCv.Domain.Entities.Obj;

namespace topCv.Application.Mappings
{

    public static class FollowCompanyMapping
    {
        public static FollowCompanyResponse ToResponse(this FollowCompany entity)
            => new()
            {
                CompanyId = entity.CompanyId,
                CompanyName = entity.Company?.Name ?? string.Empty,
                LogoUrl = entity.Company?.LogoUrl,
                FollowedAt = entity.CreatedAt
            };
    }
}
