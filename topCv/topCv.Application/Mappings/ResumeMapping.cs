using topCv.Application.DTOs.Commons;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Mappings
{
    public static class ResumeMapping
    {
        public static ResumeSummaryResponse ToSummaryResponse(this Resume entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                TemplateKey = entity.TemplateKey,
                TemplateVariantId = entity.TemplateVariantId,
                TemplateVariantKey = entity.TemplateVariant?.VariantKey,
                ThemePresetId = entity.ThemePresetId,
                ThemePresetKey = entity.ThemePreset?.ThemeKey,
                Visibility = entity.Visibility,
                IsDefault = entity.IsDefault,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

        public static ResumeSectionResponse ToResponse(this ResumeSection entity)
            => new()
            {
                Id = entity.Id,
                Type = entity.Type,
                Title = entity.Title,
                SortOrder = entity.SortOrder,
                ContentJson = entity.ContentJson,
                CreatedAt = entity.CreatedAt
            };

        public static ResumeDetailResponse ToDetailResponse(this Resume entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                TemplateKey = entity.TemplateKey,
                TemplateVariantId = entity.TemplateVariantId,
                TemplateVariantKey = entity.TemplateVariant?.VariantKey,
                ThemePresetId = entity.ThemePresetId,
                ThemePresetKey = entity.ThemePreset?.ThemeKey,
                ThemeJson = entity.ThemeJson,
                Visibility = entity.Visibility,
                IsDefault = entity.IsDefault,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Sections = entity.Sections
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.CreatedAt)
                    .Select(x => x.ToResponse())
                    .ToList(),
                Files = entity.Files
                    .OrderByDescending(x => x.UploadedAt)
                    .Select(x => x.ToResponse())
                    .ToList()
            };
    }
}
