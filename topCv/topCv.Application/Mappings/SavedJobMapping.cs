using topCv.Application.DTOs.Commons;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Mappings
{
    public static class SavedJobMapping
    {
        public static SavedJobResponse ToResponse(this SavedJob entity)
            => new()
            {
                JobId = entity.JobId,
                JobTitle = entity.Job?.Title ?? string.Empty,
                CompanyId = entity.Job?.CompanyId ?? Guid.Empty,
                CompanyName = entity.Job?.Company?.Name ?? string.Empty,
                SavedAt = entity.CreatedAt
            };
    }
}