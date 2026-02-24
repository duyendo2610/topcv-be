using topCv.Domain.Enums;

namespace topCv.Application.DTOs.Commons
{
    public sealed class UpdateJobApplicationStatusRequest
    {
        public required ApplicationStatus Status { get; init; }
        public string? Note { get; init; }
    }
}