using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Domain.Enums;

namespace topCv.Application.Services.Commons
{
    public sealed class ResumeTemplateCatalogService : IResumeTemplateCatalogService
    {
        private static readonly IReadOnlyDictionary<string, string> FamilyNames = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["simple"] = "Đơn giản",
            ["impressive"] = "Ấn tượng",
            ["professional"] = "Chuyên nghiệp",
            ["harvard"] = "Harvard"
        };

        private readonly IAppDbContext _db;

        public ResumeTemplateCatalogService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<ResumeTemplateCatalogResponse> GetCatalogAsync(CancellationToken ct)
        {
            var variants = await _db.ResumeTemplateVariants
                .AsNoTracking()
                .Include(x => x.VariantThemes)
                .ThenInclude(x => x.ThemePreset)
                .Where(x => x.IsActive)
                .OrderBy(x => x.TemplateKey)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(ct);

            var families = variants
                .GroupBy(x => x.TemplateKey)
                .Select(group => new ResumeTemplateFamilyResponse
                {
                    TemplateKey = group.Key,
                    Name = ResolveFamilyName(group.Key),
                    Variants = group.Select(v => new ResumeTemplateVariantOptionResponse
                        {
                            Id = v.Id,
                            VariantKey = v.VariantKey,
                            Name = v.Name,
                            LayoutKey = v.LayoutKey,
                            Description = v.Description,
                            SortOrder = v.SortOrder,
                            EditorSections = BuildEditorSections(v.TemplateKey, v.LayoutKey),
                            Themes = v.VariantThemes
                                .Where(link => link.ThemePreset.IsActive)
                                .OrderBy(link => link.SortOrder)
                                .ThenBy(link => link.ThemePreset.SortOrder)
                                .Select(link => new ResumeThemePresetOptionResponse
                                {
                                    Id = link.ThemePresetId,
                                    ThemeKey = link.ThemePreset.ThemeKey,
                                    Name = link.ThemePreset.Name,
                                    ThemeJson = link.ThemePreset.ThemeJson,
                                    SortOrder = link.SortOrder
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .OrderBy(x => ResolveFamilyOrder(x.TemplateKey))
                .ToList();

            return new ResumeTemplateCatalogResponse
            {
                GeneratedAtUtc = DateTime.UtcNow,
                Families = families
            };
        }

        private static List<ResumeEditorSectionOptionResponse> BuildEditorSections(string templateKey, string layoutKey)
        {
            var sections = new List<ResumeEditorSectionOptionResponse>();

            sections.Add(CreateSection(
                "summary",
                ResumeSectionType.Summary,
                "Mục tiêu nghề nghiệp",
                1,
                singleText: true,
                "Tóm tắt mục tiêu và định hướng",
                CreateField("objective", "Mục tiêu", "Nhập mục tiêu nghề nghiệp")));

            if (templateKey.Equals("impressive", StringComparison.OrdinalIgnoreCase) ||
                templateKey.Equals("professional", StringComparison.OrdinalIgnoreCase))
            {
                sections.Add(CreateProjectSection(2));
                sections.Add(CreateExperienceSection(3));
            }
            else
            {
                sections.Add(CreateExperienceSection(2));
                sections.Add(CreateProjectSection(3));
            }

            sections.Add(CreateEducationSection(4));
            sections.Add(CreateSkillSection(5));
            sections.Add(CreateCertificateSection(6));

            if (templateKey.Equals("harvard", StringComparison.OrdinalIgnoreCase))
            {
                sections.Add(CreateSection(
                    "activities",
                    ResumeSectionType.Other,
                    "Hoạt động",
                    7,
                    singleText: false,
                    "Mô tả đóng góp nổi bật",
                    CreateField("period", "Thời gian", "2023 - 2024"),
                    CreateField("organization", "Tổ chức", "Tên tổ chức"),
                    CreateField("position", "Vai trò", "Vai trò của bạn")));
            }
            else
            {
                sections.Add(CreateSection(
                    "references",
                    ResumeSectionType.Other,
                    "Người giới thiệu",
                    7,
                    singleText: true,
                    "Thông tin người giới thiệu",
                    CreateField("text", "Thông tin", "Tên, chức vụ, liên hệ")));
            }

            if (layoutKey.Contains("sidebar", StringComparison.OrdinalIgnoreCase))
            {
                return sections
                    .OrderBy(x => x.Id == "summary" ? 0 : x.Id == "skill" ? 1 : 2)
                    .ThenBy(x => x.SortOrder)
                    .Select((x, idx) => new ResumeEditorSectionOptionResponse
                    {
                        Id = x.Id,
                        Type = x.Type,
                        Title = x.Title,
                        SortOrder = idx + 1,
                        SingleText = x.SingleText,
                        NotePlaceholder = x.NotePlaceholder,
                        Fields = x.Fields
                    })
                    .ToList();
            }

            return sections.OrderBy(x => x.SortOrder).ToList();
        }

        private static ResumeEditorSectionOptionResponse CreateExperienceSection(int sortOrder)
            => CreateSection(
                "experience",
                ResumeSectionType.Experience,
                "Kinh nghiệm làm việc",
                sortOrder,
                singleText: false,
                "Mô tả kinh nghiệm và kết quả đạt được",
                CreateField("period", "Thời gian", "2022 - Hiện tại"),
                CreateField("company", "Công ty", "Tên công ty"),
                CreateField("position", "Vị trí", "Vị trí công việc"));

        private static ResumeEditorSectionOptionResponse CreateProjectSection(int sortOrder)
            => CreateSection(
                "project",
                ResumeSectionType.Project,
                "Dự án",
                sortOrder,
                singleText: false,
                "Mô tả mục tiêu, công nghệ, vai trò",
                CreateField("name", "Tên dự án", "Tên dự án"),
                CreateField("role", "Vai trò", "Vai trò của bạn"),
                CreateField("period", "Thời gian", "3/2024 - 6/2024"));

        private static ResumeEditorSectionOptionResponse CreateEducationSection(int sortOrder)
            => CreateSection(
                "education",
                ResumeSectionType.Education,
                "Học vấn",
                sortOrder,
                singleText: false,
                "Mô tả thành tích học tập",
                CreateField("period", "Thời gian", "2019 - 2023"),
                CreateField("school", "Trường học", "Tên trường"),
                CreateField("major", "Ngành học", "Công nghệ thông tin"));

        private static ResumeEditorSectionOptionResponse CreateSkillSection(int sortOrder)
            => CreateSection(
                "skill",
                ResumeSectionType.Skill,
                "Kỹ năng",
                sortOrder,
                singleText: false,
                "Mô tả mức độ thành thạo",
                CreateField("name", "Tên kỹ năng", "React"),
                CreateField("level", "Mức độ", "Nâng cao"));

        private static ResumeEditorSectionOptionResponse CreateCertificateSection(int sortOrder)
            => CreateSection(
                "certificate",
                ResumeSectionType.Certificate,
                "Chứng chỉ",
                sortOrder,
                singleText: false,
                null,
                CreateField("time", "Thời gian", "2024"),
                CreateField("name", "Tên chứng chỉ", "AWS Certified"));

        private static ResumeEditorSectionOptionResponse CreateSection(
            string id,
            ResumeSectionType type,
            string title,
            int sortOrder,
            bool singleText,
            string? notePlaceholder,
            params ResumeEditorFieldOptionResponse[] fields)
            => new()
            {
                Id = id,
                Type = type,
                Title = title,
                SortOrder = sortOrder,
                SingleText = singleText,
                NotePlaceholder = notePlaceholder,
                Fields = fields.ToList()
            };

        private static ResumeEditorFieldOptionResponse CreateField(
            string key,
            string label,
            string? placeholder = null)
            => new()
            {
                Key = key,
                Label = label,
                Placeholder = placeholder
            };

        private static string ResolveFamilyName(string templateKey)
            => FamilyNames.TryGetValue(templateKey, out var name) ? name : templateKey;

        private static int ResolveFamilyOrder(string templateKey)
            => templateKey.ToLowerInvariant() switch
            {
                "simple" => 1,
                "impressive" => 2,
                "professional" => 3,
                "harvard" => 4,
                _ => 100
            };
    }
}
