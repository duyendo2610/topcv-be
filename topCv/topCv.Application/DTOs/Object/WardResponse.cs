using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.DTOs.Object
{
    public sealed class WardResponse
    {
        public int Id { get; set; }

        public int Code { get; set; }
        public string Name { get; set; } = default!;
        public string DivisionType { get; set; } = default!;
        public string Codename { get; set; } = default!;
        public string ShortCodename { get; set; } = default!;
    }
}
