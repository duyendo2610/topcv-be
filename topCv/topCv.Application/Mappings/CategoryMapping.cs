using topCv.Application.DTOs.Commons;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Mappings
{
    public static class CategoryMapping
    {
        public static Category ToEntity(this CreateCategoryRequest req)
            => new()
            {
                Name = req.Name.Trim(),
                ParentId = req.ParentId
            };

        public static CategoryResponse ToResponse(this Category entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                ParentId = entity.ParentId
            };

        public static void ApplyTo(this UpdateCategoryRequest req, Category entity)
        {
            entity.Name = req.Name.Trim();
            entity.ParentId = req.ParentId;
        }
    }
}