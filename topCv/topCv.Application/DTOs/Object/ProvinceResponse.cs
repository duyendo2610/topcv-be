using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Obj;
using topCv.Domain.Entities.Object;

namespace topCv.Application.DTOs.Object
{
    public sealed class ProvinceResponse
    {
        public int Id { get; set; }

        public int Code { get; set; }
        public string Name { get; set; } = default!;
        public string DivisionType { get; set; } = default!;
        public string Codename { get; set; } = default!;
        public int PhoneCode { get; set; }

        public List<WardResponse> Wards { get; set; } = new();
    }
}


