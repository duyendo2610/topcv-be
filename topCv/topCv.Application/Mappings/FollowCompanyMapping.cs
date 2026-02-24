using topCv.Application.DTOs.Commons;
using topCv.Domain.Entities.Commons;

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