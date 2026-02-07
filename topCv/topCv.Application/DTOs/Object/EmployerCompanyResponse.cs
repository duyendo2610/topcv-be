using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.DTOs.Obj
{
    public sealed class EmployerCompanyResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
    }
}
