using System.Text.Json;
using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class ResumeSectionInput
    {
        public ResumeSectionType Type { get; init; }
        public string? Title { get; init; }
        public int SortOrder { get; init; } = 0;
        public JsonElement Content { get; init; }
    }
}
