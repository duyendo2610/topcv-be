namespace topCv.Domain.Entities.Commons
{
    public class ResumeTemplateVariant
    {
        public Guid Id { get; set; }
        public string TemplateKey { get; set; } = default!;
        public string VariantKey { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string LayoutKey { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public ICollection<ResumeTemplateVariantTheme> VariantThemes { get; set; } = new List<ResumeTemplateVariantTheme>();
        public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    }
}
