using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class ResumeSummaryResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string TemplateKey { get; init; } = null!;
        public Guid? TemplateVariantId { get; init; }
        public string? TemplateVariantKey { get; init; }
        public Guid? ThemePresetId { get; init; }
        public string? ThemePresetKey { get; init; }
        public ResumeVisibility Visibility { get; init; }
        public bool IsDefault { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
