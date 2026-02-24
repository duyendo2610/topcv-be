using topCv.Domain.Enums;

namespace topCv.Domain.Entities.Commons
{
    public class ResumeSection
    {
        public Guid Id { get; set; }
        public Guid ResumeId { get; set; }
        public ResumeSectionType Type { get; set; }
        public string? Title { get; set; }
        public int SortOrder { get; set; } = 0;
        public string ContentJson { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public Resume Resume { get; set; } = default!;
    }
}