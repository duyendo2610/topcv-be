namespace topCv.Domain.Entities.Commons
{
    public class JobSkill
    {
        public Guid JobId { get; set; }
        public int SkillId { get; set; }
        public Job Job { get; set; } = default!;
        public Skill Skill { get; set; } = default!;
    }
}