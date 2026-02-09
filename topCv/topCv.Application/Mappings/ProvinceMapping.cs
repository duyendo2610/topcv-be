using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;
using topCv.Application.DTOs.Object;
using topCv.Domain.Entities.Obj;
using topCv.Domain.Entities.Object;

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


