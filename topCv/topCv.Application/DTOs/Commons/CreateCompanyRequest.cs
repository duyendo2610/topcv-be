using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class CreateCompanyRequest
    {
        public required string Name { get; init; }
        public string? TaxCode { get; init; }
        public string? Website { get; init; }
        public CompanySize? Size { get; init; }
        public string? Description { get; init; }
        public int? CityId { get; init; }
        public string? Address { get; init; }
        public string? LogoUrl { get; init; }
        public string? CoverUrl { get; init; }
    }
}