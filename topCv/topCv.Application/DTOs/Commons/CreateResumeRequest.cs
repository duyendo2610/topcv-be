using System.Text.Json;
using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class CreateResumeRequest
    {
        public required string Name { get; init; }
        public ResumeVisibility Visibility { get; init; } = ResumeVisibility.Private;
        public bool IsDefault { get; init; } = false;
        public string? TemplateKey { get; init; }
        public Guid? TemplateVariantId { get; init; }
        public Guid? ThemePresetId { get; init; }
        public JsonElement? Theme { get; init; }
        public List<ResumeSectionInput> Sections { get; init; } = [];
    }
}
