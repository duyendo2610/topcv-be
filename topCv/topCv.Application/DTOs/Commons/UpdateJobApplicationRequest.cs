namespace topCv.Application.DTOs.Commons
{
    public sealed class UpdateJobApplicationRequest
    {
        public Guid? ResumeId { get; init; }
        public Guid? ResumeFileId { get; init; }
        public string? CoverLetter { get; init; }
    }
}