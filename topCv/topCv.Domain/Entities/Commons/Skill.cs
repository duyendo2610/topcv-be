namespace topCv.Domain.Entities.Commons
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }
}