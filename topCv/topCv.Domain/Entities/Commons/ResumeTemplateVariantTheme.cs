namespace topCv.Domain.Entities.Commons
{
    public class ResumeTemplateVariantTheme
    {
        public Guid VariantId { get; set; }
        public Guid ThemePresetId { get; set; }
        public int SortOrder { get; set; } = 0;
        public ResumeTemplateVariant Variant { get; set; } = default!;
        public ResumeThemePreset ThemePreset { get; set; } = default!;
    }
}
