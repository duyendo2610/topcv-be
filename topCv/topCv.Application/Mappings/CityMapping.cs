using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;
using topCv.Domain.Entities.Obj;

namespace topCv.Application.Mappings
{
    public static class CityMapping
    {
        public static City ToCity(this CreateNameRequest req)
            => new()
            {
                Name = req.Name.Trim()
            };

        public static IdNameResponse ToResponse(this City entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name
            };

        public static void ApplyTo(this UpdateNameRequest req, City entity)
        {
            entity.Name = req.Name.Trim();
        }
    }
}
