using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Domain.Entities.Obj
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public int? ParentId { get; set; }
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();

        public ICollection<JobCategory> JobCategories { get; set; } = new List<JobCategory>();
    }
}
