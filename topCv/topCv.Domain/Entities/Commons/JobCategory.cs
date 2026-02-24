namespace topCv.Domain.Entities.Commons
{
    public class JobCategory
    {
        public Guid JobId { get; set; }
        public int CategoryId { get; set; }
        public Job Job { get; set; } = default!;
        public Category Category { get; set; } = default!;
    }
}