using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class ResumeSectionResponse
    {
        public Guid Id { get; init; }
        public ResumeSectionType Type { get; init; }
        public string? Title { get; init; }
        public int SortOrder { get; init; }
        public string ContentJson { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
    }
}
