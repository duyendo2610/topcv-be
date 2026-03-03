namespace topCv.Application.DTOs.Commons
{
    public sealed class ResumeEditorFieldOptionResponse
    {
        public string Key { get; init; } = null!;
        public string Label { get; init; } = null!;
        public string? Placeholder { get; init; }
    }
}
