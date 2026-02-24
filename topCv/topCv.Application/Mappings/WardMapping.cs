using topCv.Application.DTOs.Commons;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Mappings
{
    public static class WardMapping
    {
        public static WardResponse ToResponse(this Ward ward)
        {
            return new WardResponse
            {
                Id = ward.Id,
                Code = ward.Code,
                Name = ward.Name,
                Codename = ward.Codename,
                DivisionType = ward.DivisionType,
                ShortCodename = ward.ShortCodename
            };
        }
    }
}