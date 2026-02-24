namespace topCv.Application.DTOs.Commons
{
    public sealed class CategoryTreeResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = default!;
        public int? ParentId { get; init; }
        public List<CategoryTreeResponse> Children { get; init; } = new();
    }
}