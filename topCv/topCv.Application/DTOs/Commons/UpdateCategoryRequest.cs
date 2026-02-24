namespace topCv.Application.DTOs.Commons
{
    public sealed class UpdateCategoryRequest
    {
        public required string Name { get; init; }
        public int? ParentId { get; init; }
    }
}