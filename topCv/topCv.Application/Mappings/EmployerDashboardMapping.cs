using topCv.Application.DTOs.Commons;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Mappings
{
    public static class EmployerDashboardMapping
    {
        public static EmployerCompanyResponse ToEmployerResponse(this Company entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedAt = entity.CreatedAt
            };

        public static EmployerJobResponse ToEmployerResponse(this Job entity, int totalApplications)
            => new()
            {
                Id = entity.Id,
                Title = entity.Title,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                TotalApplications = totalApplications
            };
    }
}