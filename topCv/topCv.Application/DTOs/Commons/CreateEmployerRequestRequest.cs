using System.ComponentModel.DataAnnotations;

namespace topCv.Application.DTOs.Commons
{
    public sealed class CreateEmployerRequestRequest
    {
        [Required]
        public Guid CompanyId { get; set; }

        public string? Message { get; set; }
    }
}
