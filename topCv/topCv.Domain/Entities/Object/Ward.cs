using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Obj;

namespace topCv.Domain.Entities.Object
{
    public class Ward
    {
        public int Id { get; set; }

        public int Code { get; set; }
        public string Name { get; set; } = default!;
        public string Codename { get; set; } = default!;
        public string DivisionType { get; set; } = default!;
        public string ShortCodename { get; set; } = default!;

        public int ProvinceId { get; set; }
        public Province Province { get; set; } = default!;
    }
}
