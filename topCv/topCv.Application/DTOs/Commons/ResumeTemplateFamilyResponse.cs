namespace topCv.Application.DTOs.Commons
{
    public sealed class ResumeTemplateFamilyResponse
    {
        public string TemplateKey { get; init; } = null!;
        public string Name { get; init; } = null!;
        public List<ResumeTemplateVariantOptionResponse> Variants { get; init; } = [];
    }
}
