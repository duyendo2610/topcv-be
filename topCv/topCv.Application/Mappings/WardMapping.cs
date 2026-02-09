using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Object;
using topCv.Domain.Entities.Object;

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
