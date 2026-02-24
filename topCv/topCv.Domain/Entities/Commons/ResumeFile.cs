namespace topCv.Domain.Entities.Commons
{
    public class ResumeFile
    {
        public Guid Id { get; set; }
        public Guid ResumeId { get; set; }
        public string FileUrl { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public long FileSize { get; set; }
        public string MimeType { get; set; } = default!;
        public DateTime UploadedAt { get; set; }
        public Resume Resume { get; set; } = default!;
        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    }
}