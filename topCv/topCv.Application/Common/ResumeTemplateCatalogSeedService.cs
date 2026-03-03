using Microsoft.EntityFrameworkCore;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Common
{
    public sealed class ResumeTemplateCatalogSeedService
    {
        private readonly IAppDbContext _db;

        public ResumeTemplateCatalogSeedService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var families = BuildFamilies();

            var variants = await _db.ResumeTemplateVariants.ToListAsync(ct);
            var themes = await _db.ResumeThemePresets.ToListAsync(ct);
            var links = await _db.ResumeTemplateVariantThemes.ToListAsync(ct);

            foreach (var family in families)
            {
                foreach (var variantDef in family.Variants)
                {
                    var variant = variants.FirstOrDefault(x =>
                        x.VariantKey.Equals(variantDef.VariantKey, StringComparison.OrdinalIgnoreCase));

                    if (variant is null)
                    {
                        variant = new ResumeTemplateVariant
                        {
                            Id = Guid.NewGuid(),
                            TemplateKey = family.TemplateKey,
                            VariantKey = variantDef.VariantKey,
                            Name = variantDef.Name,
                            LayoutKey = variantDef.LayoutKey,
                            Description = variantDef.Description,
                            SortOrder = variantDef.SortOrder,
                            IsActive = true,
                            CreatedAt = now
                        };
                        _db.ResumeTemplateVariants.Add(variant);
                        variants.Add(variant);
                    }
                    else
                    {
                        variant.TemplateKey = family.TemplateKey;
                        variant.Name = variantDef.Name;
                        variant.LayoutKey = variantDef.LayoutKey;
                        variant.Description = variantDef.Description;
                        variant.SortOrder = variantDef.SortOrder;
                        variant.IsActive = true;
                    }
                }

                foreach (var themeDef in family.Themes)
                {
                    var theme = themes.FirstOrDefault(x =>
                        x.ThemeKey.Equals(themeDef.ThemeKey, StringComparison.OrdinalIgnoreCase));

                    if (theme is null)
                    {
                        theme = new ResumeThemePreset
                        {
                            Id = Guid.NewGuid(),
                            ThemeKey = themeDef.ThemeKey,
                            Name = themeDef.Name,
                            TemplateKey = family.TemplateKey,
                            ThemeJson = themeDef.ThemeJson,
                            SortOrder = themeDef.SortOrder,
                            IsActive = true,
                            CreatedAt = now
                        };
                        _db.ResumeThemePresets.Add(theme);
                        themes.Add(theme);
                    }
                    else
                    {
                        theme.Name = themeDef.Name;
                        theme.TemplateKey = family.TemplateKey;
                        theme.ThemeJson = themeDef.ThemeJson;
                        theme.SortOrder = themeDef.SortOrder;
                        theme.IsActive = true;
                    }
                }
            }

            await _db.SaveChangesAsync(ct);

            foreach (var family in families)
            {
                var familyVariantIds = variants
                    .Where(x => x.TemplateKey.Equals(family.TemplateKey, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Id)
                    .ToList();
                var familyThemes = themes
                    .Where(x => x.TemplateKey != null && x.TemplateKey.Equals(family.TemplateKey, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                foreach (var variantId in familyVariantIds)
                {
                    var idx = 0;
                    foreach (var theme in familyThemes)
                    {
                        var exists = links.Any(x => x.VariantId == variantId && x.ThemePresetId == theme.Id);
                        if (exists) continue;

                        var link = new ResumeTemplateVariantTheme
                        {
                            VariantId = variantId,
                            ThemePresetId = theme.Id,
                            SortOrder = idx++
                        };
                        _db.ResumeTemplateVariantThemes.Add(link);
                        links.Add(link);
                    }
                }
            }

            await _db.SaveChangesAsync(ct);
        }

        private static List<TemplateFamilySeed> BuildFamilies()
        {
            return
            [
                new TemplateFamilySeed
                {
                    TemplateKey = "simple",
                    Variants =
                    [
                        new TemplateVariantSeed("simple_clean", "Đơn giản tinh gọn", "linear",
                            "Bố cục một cột tối giản, chỉnh sửa nhanh.", 1),
                        new TemplateVariantSeed("simple_split", "Đơn giản chia cột", "split",
                            "Bố cục hai cột cân đối, khoảng cách gọn gàng.", 2),
                        new TemplateVariantSeed("simple_focus", "Đơn giản tập trung", "focus",
                            "Bố cục tập trung theo mục với điểm nhấn nhẹ.", 3)
                    ],
                    Themes =
                    [
                        new ThemeSeed("simple_blue", "Xanh đơn giản",
                            """{"primaryColor":"#183153","accentColor":"#2f80ed","textColor":"#1f2937","mutedTextColor":"#5b667a","backgroundColor":"#ffffff","bodyFont":"modern","headingFont":"modern"}""",
                            1),
                        new ThemeSeed("simple_green", "Xanh lá đơn giản",
                            """{"primaryColor":"#1b5e20","accentColor":"#2e7d32","textColor":"#1f2937","mutedTextColor":"#5f6b7a","backgroundColor":"#ffffff","bodyFont":"modern","headingFont":"modern"}""",
                            2),
                        new ThemeSeed("simple_charcoal", "Xám than đơn giản",
                            """{"primaryColor":"#263238","accentColor":"#455a64","textColor":"#263238","mutedTextColor":"#616161","backgroundColor":"#ffffff","bodyFont":"modern","headingFont":"serif"}""",
                            3)
                    ]
                },
                new TemplateFamilySeed
                {
                    TemplateKey = "impressive",
                    Variants =
                    [
                        new TemplateVariantSeed("impressive_gradient", "Ấn tượng chuyển sắc", "gradient",
                            "Header phong cách hero và hiệu ứng chuyển sắc.", 1),
                        new TemplateVariantSeed("impressive_card", "Ấn tượng dạng thẻ", "cards",
                            "Các mục dạng thẻ với nhóm thị giác rõ ràng.", 2),
                        new TemplateVariantSeed("impressive_sidebar", "Ấn tượng có thanh bên", "sidebar",
                            "Nhấn mạnh thanh bên cho thông tin hồ sơ chính.", 3)
                    ],
                    Themes =
                    [
                        new ThemeSeed("impressive_ocean", "Nhịp sóng đại dương",
                            """{"primaryColor":"#1f4fb2","accentColor":"#2f80ed","textColor":"#13233f","mutedTextColor":"#4d5f7d","backgroundColor":"#ffffff","bodyFont":"modern","headingFont":"modern"}""",
                            1),
                        new ThemeSeed("impressive_sunrise", "Bình minh rực lửa",
                            """{"primaryColor":"#b45309","accentColor":"#ea580c","textColor":"#3f2a1a","mutedTextColor":"#7c4a2f","backgroundColor":"#fffdf8","bodyFont":"modern","headingFont":"serif"}""",
                            2),
                        new ThemeSeed("impressive_neon", "Đêm neon",
                            """{"primaryColor":"#0f172a","accentColor":"#0891b2","textColor":"#0f172a","mutedTextColor":"#334155","backgroundColor":"#f8fafc","bodyFont":"modern","headingFont":"modern"}""",
                            3)
                    ]
                },
                new TemplateFamilySeed
                {
                    TemplateKey = "professional",
                    Variants =
                    [
                        new TemplateVariantSeed("professional_standard", "Chuyên nghiệp tiêu chuẩn", "linear",
                            "Định dạng truyền thống, thân thiện với nhà tuyển dụng.", 1),
                        new TemplateVariantSeed("professional_twocol", "Chuyên nghiệp hai cột", "twocol",
                            "Bố cục hai cột gọn cho thông tin dày.", 2),
                        new TemplateVariantSeed("professional_band", "Chuyên nghiệp dải băng", "band",
                            "Dải băng trên cùng phong cách quản lý và cấu trúc mục.", 3)
                    ],
                    Themes =
                    [
                        new ThemeSeed("professional_navy", "Xanh navy doanh nghiệp",
                            """{"primaryColor":"#1e3a8a","accentColor":"#334155","textColor":"#111827","mutedTextColor":"#4b5563","backgroundColor":"#ffffff","bodyFont":"modern","headingFont":"serif"}""",
                            1),
                        new ThemeSeed("professional_slate", "Xám slate văn phòng",
                            """{"primaryColor":"#1f2937","accentColor":"#475569","textColor":"#111827","mutedTextColor":"#4b5563","backgroundColor":"#ffffff","bodyFont":"modern","headingFont":"serif"}""",
                            2),
                        new ThemeSeed("professional_emerald", "Xanh ngọc quản lý",
                            """{"primaryColor":"#065f46","accentColor":"#047857","textColor":"#1f2937","mutedTextColor":"#4b5563","backgroundColor":"#ffffff","bodyFont":"modern","headingFont":"serif"}""",
                            3)
                    ]
                },
                new TemplateFamilySeed
                {
                    TemplateKey = "harvard",
                    Variants =
                    [
                        new TemplateVariantSeed("harvard_classic", "Harvard cổ điển", "classic",
                            "Phong cách CV học thuật với font serif.", 1),
                        new TemplateVariantSeed("harvard_compact", "Harvard gọn", "compact",
                            "Cấu trúc cô đọng, gọn cho hồ sơ nhiều ấn phẩm.", 2),
                        new TemplateVariantSeed("harvard_annotated", "Harvard có chú thích", "annotated",
                            "Khối mục có chú thích cho thành tựu nghiên cứu.", 3)
                    ],
                    Themes =
                    [
                        new ThemeSeed("harvard_maroon", "Harvard đỏ mận",
                            """{"primaryColor":"#7c2d12","accentColor":"#9a3412","textColor":"#292524","mutedTextColor":"#57534e","backgroundColor":"#fffdf8","bodyFont":"classic","headingFont":"serif"}""",
                            1),
                        new ThemeSeed("harvard_sepia", "Harvard nâu sepia",
                            """{"primaryColor":"#78350f","accentColor":"#92400e","textColor":"#292524","mutedTextColor":"#57534e","backgroundColor":"#fffbeb","bodyFont":"classic","headingFont":"serif"}""",
                            2),
                        new ThemeSeed("harvard_ink", "Harvard mực đen",
                            """{"primaryColor":"#1e293b","accentColor":"#334155","textColor":"#0f172a","mutedTextColor":"#475569","backgroundColor":"#ffffff","bodyFont":"classic","headingFont":"serif"}""",
                            3)
                    ]
                }
            ];
        }

        private sealed class TemplateFamilySeed
        {
            public required string TemplateKey { get; init; }
            public required List<TemplateVariantSeed> Variants { get; init; }
            public required List<ThemeSeed> Themes { get; init; }
        }

        private sealed record TemplateVariantSeed(
            string VariantKey,
            string Name,
            string LayoutKey,
            string Description,
            int SortOrder);

        private sealed record ThemeSeed(
            string ThemeKey,
            string Name,
            string ThemeJson,
            int SortOrder);
    }
}
