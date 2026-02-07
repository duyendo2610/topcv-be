using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Obj
{
    public sealed class UpdateCompanyRequest
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
