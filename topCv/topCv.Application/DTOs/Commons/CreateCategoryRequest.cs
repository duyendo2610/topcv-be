namespace topCv.Application.DTOs.Commons
{
    public sealed class CreateCategoryRequest
    {
        public required string Name { get; init; }
        public int? ParentId { get; init; }
    }
}