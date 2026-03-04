namespace topCv.Application.DTOs.Commons
{
    public sealed class ResumeThemePresetOptionResponse
    {
        public Guid Id { get; init; }
        public string ThemeKey { get; init; } = null!;
        public string Name { get; init; } = null!;
        public string ThemeJson { get; init; } = "{}";
        public int SortOrder { get; init; }
    }
}
