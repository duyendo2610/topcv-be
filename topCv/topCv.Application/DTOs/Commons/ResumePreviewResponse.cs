namespace topCv.Application.DTOs.Commons
{
    public sealed class ResumePreviewResponse
    {
        public Guid ResumeId { get; init; }
        public string ResumeName { get; init; } = null!;
        public string TemplateKey { get; init; } = null!;
        public string? TemplateVariantKey { get; init; }
        public string? ThemePresetKey { get; init; }
        public string Html { get; init; } = null!;
        public DateTime GeneratedAtUtc { get; init; }
    }
}
