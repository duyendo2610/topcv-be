namespace topCv.Application.DTOs.Commons
{
    public sealed class CategoryResponse
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public int? ParentId { get; init; }
    }
}