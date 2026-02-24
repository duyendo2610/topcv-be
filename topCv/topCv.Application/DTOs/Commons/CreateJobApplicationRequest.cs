namespace topCv.Application.DTOs.Commons
{
    public sealed class CreateJobApplicationRequest
    {
        public required Guid JobId { get; init; }
        public required Guid ResumeId { get; init; }
        public Guid? ResumeFileId { get; init; }
        public string? CoverLetter { get; init; }
    }
}