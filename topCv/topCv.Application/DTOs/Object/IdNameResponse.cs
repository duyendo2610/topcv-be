using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.DTOs.Obj
{
    public sealed class IdNameResponse
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
    }
}
