namespace topCv.Application.DTOs.Commons
{
    public sealed class ResumeTemplateVariantOptionResponse
    {
        public Guid Id { get; init; }
        public string VariantKey { get; init; } = null!;
        public string Name { get; init; } = null!;
        public string LayoutKey { get; init; } = null!;
        public string? Description { get; init; }
        public int SortOrder { get; init; }
        public List<ResumeThemePresetOptionResponse> Themes { get; init; } = [];
        public List<ResumeEditorSectionOptionResponse> EditorSections { get; init; } = [];
    }
}
