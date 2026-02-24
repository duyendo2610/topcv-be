using topCv.Application.DTOs.Commons;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Mappings
{
    public static class ProvinceMapping
    {
        public static ProvinceResponse ToResponse(this Province province)
        {
            return new ProvinceResponse
            {
                Id = province.Id,
                Code = province.Code,
                Name = province.Name,
                DivisionType = province.DivisionType,
                Codename = province.Codename,
                PhoneCode = province.PhoneCode,

                Wards = province.Wards
                    .Select(w => w.ToResponse())
                    .ToList()
            };
        }
    }
}