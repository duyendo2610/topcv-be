using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.DTOs.Obj
{
    public sealed class CategoryTreeResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = default!;
        public int? ParentId { get; init; }

        public List<CategoryTreeResponse> Children { get; init; } = new();
    }
}
