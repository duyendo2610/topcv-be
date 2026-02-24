using topCv.Application.DTOs.Commons;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Mappings
{
    public static class CompanyMapping
    {
        public static Company ToCompany(this CreateCompanyRequest req, Guid ownerUserId)
            => new()
            {
                Id = Guid.NewGuid(),
                OwnerUserId = ownerUserId,
                Name = req.Name.Trim(),
                TaxCode = req.TaxCode,
                Website = req.Website,
                Size = req.Size,
                Description = req.Description,
                CityId = req.CityId,
                Address = req.Address,
                LogoUrl = req.LogoUrl,
                CoverUrl = req.CoverUrl,
                IsVerified = false
            };

        public static CompanyResponse ToResponse(this Company entity)
            => new()
            {
                Id = entity.Id,
                OwnerUserId = entity.OwnerUserId,
                Name = entity.Name,
                TaxCode = entity.TaxCode,
                Website = entity.Website,
                Size = entity.Size,
                Description = entity.Description,
                CityId = entity.CityId,
                CityName = entity.Province?.Name,
                Address = entity.Address,
                LogoUrl = entity.LogoUrl,
                CoverUrl = entity.CoverUrl,
                IsVerified = entity.IsVerified
            };

        public static void ApplyTo(this UpdateCompanyRequest req, Company entity)
        {
            entity.Name = req.Name.Trim();
            entity.TaxCode = req.TaxCode;
            entity.Website = req.Website;
            entity.Size = req.Size;
            entity.Description = req.Description;
            entity.CityId = req.CityId;
            entity.Address = req.Address;
            entity.LogoUrl = req.LogoUrl;
            entity.CoverUrl = req.CoverUrl;
        }
    }
}