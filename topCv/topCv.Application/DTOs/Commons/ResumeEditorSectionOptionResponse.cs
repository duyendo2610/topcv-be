using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class ResumeEditorSectionOptionResponse
    {
        public string Id { get; init; } = null!;
        public ResumeSectionType Type { get; init; }
        public string Title { get; init; } = null!;
        public int SortOrder { get; init; }
        public bool SingleText { get; init; }
        public string? NotePlaceholder { get; init; }
        public List<ResumeEditorFieldOptionResponse> Fields { get; init; } = [];
    }
}
