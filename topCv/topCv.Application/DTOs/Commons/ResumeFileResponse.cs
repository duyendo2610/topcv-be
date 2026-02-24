namespace topCv.Application.DTOs.Commons
{
    public sealed class ResumeFileResponse
    {
        public Guid Id { get; init; }
        public Guid ResumeId { get; init; }
        public string FileUrl { get; init; } = null!;
        public string FileName { get; init; } = null!;
        public string MimeType { get; init; } = null!;
        public long FileSize { get; init; }
        public DateTime UploadedAt { get; init; }
    }
}