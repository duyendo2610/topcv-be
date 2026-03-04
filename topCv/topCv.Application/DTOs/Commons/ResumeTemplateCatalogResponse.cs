namespace topCv.Application.DTOs.Commons
{
    public sealed class ResumeTemplateCatalogResponse
    {
        public DateTime GeneratedAtUtc { get; init; }
        public List<ResumeTemplateFamilyResponse> Families { get; init; } = [];
    }
}
