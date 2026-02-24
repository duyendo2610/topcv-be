namespace topCv.Application.Common
{
    public class ProvinceApiDto
    {
        public string Name { get; set; } = default!;
        public int Code { get; set; }
        public string Codename { get; set; } = default!;
        public string Division_Type { get; set; } = default!;
        public int Phone_Code { get; set; }

        public List<WardApiDto> Wards { get; set; } = new();
    }
}