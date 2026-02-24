namespace topCv.Application.DTOs.Commons
{
    public sealed class WardResponse
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; } = default!;
        public string DivisionType { get; set; } = default!;
        public string Codename { get; set; } = default!;
        public string ShortCodename { get; set; } = default!;
    }
}