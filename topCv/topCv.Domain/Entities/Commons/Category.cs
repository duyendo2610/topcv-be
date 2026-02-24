namespace topCv.Domain.Entities.Commons
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