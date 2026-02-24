namespace topCv.Domain.Entities.Commons
{
    public class Province
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; } = default!;
        public string DivisionType { get; set; } = default!;
        public string Codename { get; set; } = default!;
        public int PhoneCode { get; set; }
        public ICollection<Ward> Wards { get; set; } = new List<Ward>();
        public ICollection<CandidateProfile> CandidateProfiles { get; set; } = new List<CandidateProfile>();
        public ICollection<Company> Companies { get; set; } = new List<Company>();
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}