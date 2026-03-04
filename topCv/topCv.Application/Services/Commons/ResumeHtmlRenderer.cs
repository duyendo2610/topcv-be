using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Services.Commons
{
    internal static class ResumeHtmlRenderer
    {
        private static readonly Regex SafeColorRegex = new("^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$",
            RegexOptions.Compiled);
        private static readonly Regex SafeKeyRegex = new("^[a-z0-9_-]{2,80}$", RegexOptions.Compiled);

        public static string Render(
            Resume resume,
            IReadOnlyList<ResumeSection> sections,
            ResumeTemplateVariant? variant,
            ResumeThemePreset? themePreset)
        {
            var templateKey = NormalizeKey(variant?.TemplateKey ?? resume.TemplateKey, "simple");
            var variantKey = NormalizeKey(variant?.VariantKey, $"{templateKey}_v1");
            var theme = ParseTheme(templateKey, themePreset?.ThemeJson, resume.ThemeJson);
            var safeTitle = HtmlEncode(string.IsNullOrWhiteSpace(resume.Name) ? "CV chưa đặt tên" : resume.Name);
            var renderedSections = sections
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.CreatedAt)
                .Select(RenderSection)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("<!doctype html>");
            sb.AppendLine("<html lang=\"vi\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"utf-8\" />");
            sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
            sb.AppendLine($"  <title>{safeTitle}</title>");
            sb.AppendLine("  <style>");
            sb.AppendLine("    :root {");
            sb.AppendLine($"      --primary: {theme.PrimaryColor};");
            sb.AppendLine($"      --accent: {theme.AccentColor};");
            sb.AppendLine($"      --text: {theme.TextColor};");
            sb.AppendLine($"      --muted: {theme.MutedTextColor};");
            sb.AppendLine($"      --paper: {theme.BackgroundColor};");
            sb.AppendLine($"      --font-body: {theme.BodyFontFamily};");
            sb.AppendLine($"      --font-heading: {theme.HeadingFontFamily};");
            sb.AppendLine("    }");
            sb.AppendLine("    * { box-sizing: border-box; }");
            sb.AppendLine("    body { margin: 0; padding: 24px; color: var(--text); font-family: var(--font-body); }");
            sb.AppendLine(
                "    .page { max-width: 920px; margin: 0 auto; background: var(--paper); border: 1px solid #d8dee9; box-shadow: 0 14px 34px rgba(17, 28, 45, 0.08); }");
            sb.AppendLine("    .header { padding: 28px 32px 18px; border-bottom: 3px solid var(--primary); }");
            sb.AppendLine(
                "    .title { margin: 0; color: var(--primary); font-family: var(--font-heading); font-size: 30px; line-height: 1.2; letter-spacing: 0.2px; }");
            sb.AppendLine("    .meta { margin-top: 8px; color: var(--muted); font-size: 13px; }");
            sb.AppendLine("    .content { padding: 20px 32px 28px; }");
            sb.AppendLine("    section { margin-bottom: 20px; }");
            sb.AppendLine(
                "    section h2 { margin: 0 0 8px; font-family: var(--font-heading); font-size: 17px; color: var(--primary); border-bottom: 1px solid #e2e8f3; padding-bottom: 6px; }");
            sb.AppendLine("    p { margin: 0 0 8px; line-height: 1.55; }");
            sb.AppendLine("    .muted { color: var(--muted); }");
            sb.AppendLine("    .kv { display: grid; grid-template-columns: 160px 1fr; gap: 8px; margin-bottom: 8px; }");
            sb.AppendLine("    .k { color: var(--muted); font-size: 13px; }");
            sb.AppendLine("    .v { color: var(--text); }");
            sb.AppendLine("    .group { margin-bottom: 10px; }");
            sb.AppendLine("    .group-title { color: var(--accent); font-weight: 600; margin-bottom: 4px; }");
            sb.AppendLine("    ul.list { margin: 0 0 10px 18px; padding: 0; }");
            sb.AppendLine("    ul.list li { margin: 4px 0; line-height: 1.45; }");
            sb.AppendLine("    .card { border: 1px solid #e2e8f3; border-radius: 8px; padding: 12px; background: #fff; }");
            sb.AppendLine("    .split { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }");
            sb.AppendLine("    .split-col > section { margin-bottom: 16px; }");
            sb.AppendLine("    .with-sidebar { display: grid; grid-template-columns: 280px 1fr; gap: 18px; }");
            sb.AppendLine("    .sidebar { background: rgba(0,0,0,0.02); border: 1px solid #e2e8f3; padding: 12px; }");
            sb.AppendLine("    .sidebar .card { border: none; padding: 0; }");
            sb.AppendLine("    .section-index { color: var(--muted); font-size: 12px; margin-bottom: 6px; }");
            sb.AppendLine("    .template-simple body, .template-simple { background: #f7fafc; }");
            sb.AppendLine("    .template-simple .header { background: linear-gradient(90deg, #ffffff 20%, #f4f8ff 100%); }");
            sb.AppendLine("    .template-simple.variant-simple_split .content { padding-top: 14px; }");
            sb.AppendLine("    .template-simple.variant-simple_focus section { border-left: 4px solid #dbeafe; padding-left: 12px; }");
            sb.AppendLine("    .template-impressive body, .template-impressive { background: #eaf2ff; }");
            sb.AppendLine(
                "    .template-impressive .header { background: radial-gradient(circle at top right, rgba(47,128,237,0.18), rgba(0,0,0,0) 40%); border-bottom-width: 4px; }");
            sb.AppendLine("    .template-impressive .title { font-size: 34px; letter-spacing: 0.4px; }");
            sb.AppendLine(
                "    .template-impressive.variant-impressive_gradient .page { background: linear-gradient(180deg, #ffffff, #f7fbff); }");
            sb.AppendLine("    .template-impressive.variant-impressive_card section { border: 1px solid #dce6f7; border-radius: 10px; padding: 10px; }");
            sb.AppendLine("    .template-impressive.variant-impressive_sidebar .sidebar { background: #f2f7ff; }");
            sb.AppendLine("    .template-professional body, .template-professional { background: #f2f4f7; }");
            sb.AppendLine("    .template-professional .header { border-bottom: 2px solid #94a3b8; }");
            sb.AppendLine("    .template-professional .title { font-size: 29px; }");
            sb.AppendLine("    .template-professional section h2 { text-transform: uppercase; font-size: 14px; letter-spacing: 0.5px; }");
            sb.AppendLine("    .template-professional.variant-professional_twocol .kv { grid-template-columns: 130px 1fr; }");
            sb.AppendLine(
                "    .template-professional.variant-professional_band .header { background: linear-gradient(90deg, rgba(24,49,83,0.09), rgba(24,49,83,0)); }");
            sb.AppendLine("    .template-harvard body, .template-harvard { background: #faf7f2; }");
            sb.AppendLine("    .template-harvard .page { border-color: #d7cec3; }");
            sb.AppendLine("    .template-harvard .header { border-bottom-color: #7c4f2f; }");
            sb.AppendLine(
                "    .template-harvard .title { color: #7c4f2f; font-family: \"Merriweather\", Georgia, serif; font-size: 28px; }");
            sb.AppendLine("    .template-harvard section h2 { color: #7c4f2f; border-bottom-color: #dbcbbf; }");
            sb.AppendLine("    .template-harvard.variant-harvard_compact .content { padding: 16px 24px 20px; }");
            sb.AppendLine("    .template-harvard.variant-harvard_compact .title { font-size: 24px; }");
            sb.AppendLine("    .template-harvard.variant-harvard_annotated section { border-left: 3px solid #d9c4b2; padding-left: 10px; }");
            sb.AppendLine(
                "    @media (max-width: 860px) { .split, .with-sidebar { grid-template-columns: 1fr; } .content { padding: 16px 20px 20px; } .header { padding: 20px; } }");
            sb.AppendLine(
                "    @media print { body { background: #fff; padding: 0; } .page { box-shadow: none; border: none; max-width: none; } @page { size: A4; margin: 12mm; } }");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine($"<body><article class=\"page template-{templateKey} variant-{variantKey}\">");
            sb.AppendLine("  <header class=\"header\">");
            sb.AppendLine($"    <h1 class=\"title\">{safeTitle}</h1>");
            sb.AppendLine($"    <div class=\"meta\">Mẫu: {HtmlEncode(templateKey)} / {HtmlEncode(variantKey)}</div>");
            sb.AppendLine("  </header>");
            sb.AppendLine("  <main class=\"content\">");
            sb.AppendLine(RenderSectionsLayout(variantKey, renderedSections));
            sb.AppendLine("  </main>");
            sb.AppendLine("</article></body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private static string RenderSectionsLayout(string variantKey, IReadOnlyList<string> sections)
        {
            if (sections.Count == 0)
            {
                return "<p class=\"muted\">CV này chưa có mục nào.</p>";
            }

            var lower = variantKey.ToLowerInvariant();
            if (lower.Contains("sidebar"))
            {
                var sidebar = sections.First();
                var main = sections.Skip(1).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("<div class=\"with-sidebar\">");
                sb.AppendLine("  <aside class=\"sidebar\">");
                sb.AppendLine(sidebar);
                sb.AppendLine("  </aside>");
                sb.AppendLine("  <div>");
                foreach (var section in main)
                {
                    sb.AppendLine(section);
                }

                sb.AppendLine("  </div>");
                sb.AppendLine("</div>");
                return sb.ToString();
            }

            if (lower.Contains("split") || lower.Contains("twocol"))
            {
                var left = sections.Where((_, idx) => idx % 2 == 0).ToList();
                var right = sections.Where((_, idx) => idx % 2 == 1).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("<div class=\"split\">");
                sb.AppendLine("  <div class=\"split-col\">");
                foreach (var section in left)
                {
                    sb.AppendLine(section);
                }

                sb.AppendLine("  </div>");
                sb.AppendLine("  <div class=\"split-col\">");
                foreach (var section in right)
                {
                    sb.AppendLine(section);
                }

                sb.AppendLine("  </div>");
                sb.AppendLine("</div>");
                return sb.ToString();
            }

            if (lower.Contains("annotated"))
            {
                var sb = new StringBuilder();
                for (var i = 0; i < sections.Count; i++)
                {
                    sb.AppendLine("<div class=\"card\">");
                    sb.AppendLine($"<div class=\"section-index\">Mục {i + 1}</div>");
                    sb.AppendLine(sections[i]);
                    sb.AppendLine("</div>");
                }

                return sb.ToString();
            }

            return string.Join(Environment.NewLine, sections);
        }

        private static string RenderSection(ResumeSection section)
        {
            var title = string.IsNullOrWhiteSpace(section.Title) ? section.Type.ToString() : section.Title.Trim();
            var safeTitle = HtmlEncode(title);
            var body = RenderContent(section.ContentJson);
            return $"""
                    <section>
                      <h2>{safeTitle}</h2>
                      {body}
                    </section>
                    """;
        }

        private static string RenderContent(string contentJson)
        {
            if (string.IsNullOrWhiteSpace(contentJson))
            {
                return "<p class=\"muted\">Không có nội dung.</p>";
            }

            try
            {
                using var doc = JsonDocument.Parse(contentJson);
                return RenderJsonNode(doc.RootElement);
            }
            catch
            {
                return $"<p>{HtmlEncode(contentJson)}</p>";
            }
        }

        private static string RenderJsonNode(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return $"<p>{HtmlEncode(element.GetString() ?? string.Empty)}</p>";
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return $"<p>{HtmlEncode(element.ToString())}</p>";
                case JsonValueKind.Object:
                    return RenderJsonObject(element);
                case JsonValueKind.Array:
                    return RenderJsonArray(element);
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return "<p class=\"muted\">-</p>";
                default:
                    return "<p class=\"muted\">Nội dung không được hỗ trợ.</p>";
            }
        }

        private static string RenderJsonObject(JsonElement element)
        {
            var props = element.EnumerateObject().ToList();
            if (props.Count == 0)
            {
                return "<p class=\"muted\">Không có chi tiết.</p>";
            }

            var sb = new StringBuilder();
            foreach (var prop in props)
            {
                var key = HtmlEncode(ToLabel(prop.Name));
                if (IsPrimitive(prop.Value))
                {
                    sb.AppendLine(
                        $"<div class=\"kv\"><span class=\"k\">{key}</span><span class=\"v\">{HtmlEncode(ValueToString(prop.Value))}</span></div>");
                }
                else
                {
                    sb.AppendLine("<div class=\"group\">");
                    sb.AppendLine($"  <div class=\"group-title\">{key}</div>");
                    sb.AppendLine(RenderJsonNode(prop.Value));
                    sb.AppendLine("</div>");
                }
            }

            return sb.ToString();
        }

        private static string RenderJsonArray(JsonElement element)
        {
            var arr = element.EnumerateArray().ToList();
            if (arr.Count == 0)
            {
                return "<p class=\"muted\">Không có mục nào.</p>";
            }

            var sb = new StringBuilder();
            sb.AppendLine("<ul class=\"list\">");
            foreach (var item in arr)
            {
                if (IsPrimitive(item))
                {
                    sb.AppendLine($"  <li>{HtmlEncode(ValueToString(item))}</li>");
                    continue;
                }

                sb.AppendLine("  <li>");
                sb.AppendLine(RenderJsonNode(item));
                sb.AppendLine("  </li>");
            }

            sb.AppendLine("</ul>");
            return sb.ToString();
        }

        private static bool IsPrimitive(JsonElement value)
            => value.ValueKind is JsonValueKind.String or JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False or
                JsonValueKind.Null or JsonValueKind.Undefined;

        private static string ValueToString(JsonElement value)
            => value.ValueKind switch
            {
                JsonValueKind.String => value.GetString() ?? string.Empty,
                JsonValueKind.Null => "-",
                JsonValueKind.Undefined => "-",
                _ => value.ToString()
            };

        private static string ToLabel(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return "Trường";
            var withSpaces = Regex.Replace(key, "([a-z])([A-Z])", "$1 $2");
            return char.ToUpperInvariant(withSpaces[0]) + withSpaces[1..];
        }

        private static string HtmlEncode(string text) => WebUtility.HtmlEncode(text);

        private static string NormalizeKey(string? key, string fallback)
        {
            if (string.IsNullOrWhiteSpace(key)) return fallback;
            var normalized = key.Trim().ToLowerInvariant();
            return SafeKeyRegex.IsMatch(normalized) ? normalized : fallback;
        }

        private static ResumeTheme ParseTheme(string templateKey, string? presetThemeJson, string? overrideThemeJson)
        {
            var theme = ResumeTheme.Default(templateKey);
            ApplyThemeJson(theme, presetThemeJson);
            ApplyThemeJson(theme, overrideThemeJson);
            return theme;
        }

        private static void ApplyThemeJson(ResumeTheme theme, string? themeJson)
        {
            if (string.IsNullOrWhiteSpace(themeJson))
            {
                return;
            }

            try
            {
                using var doc = JsonDocument.Parse(themeJson);
                if (doc.RootElement.ValueKind != JsonValueKind.Object) return;

                var root = doc.RootElement;
                theme.PrimaryColor = ReadColor(root, "primaryColor", theme.PrimaryColor);
                theme.AccentColor = ReadColor(root, "accentColor", theme.AccentColor);
                theme.TextColor = ReadColor(root, "textColor", theme.TextColor);
                theme.MutedTextColor = ReadColor(root, "mutedTextColor", theme.MutedTextColor);
                theme.BackgroundColor = ReadColor(root, "backgroundColor", theme.BackgroundColor);
                theme.BodyFontFamily = ReadFontFamily(root, "bodyFont", theme.BodyFontFamily);
                theme.HeadingFontFamily = ReadFontFamily(root, "headingFont", theme.HeadingFontFamily);
            }
            catch
            {
                // ignore malformed theme json and keep previous values
            }
        }

        private static string ReadColor(JsonElement root, string key, string fallback)
        {
            if (!root.TryGetProperty(key, out var value) || value.ValueKind != JsonValueKind.String)
            {
                return fallback;
            }

            var raw = value.GetString()?.Trim() ?? string.Empty;
            return SafeColorRegex.IsMatch(raw) ? raw : fallback;
        }

        private static string ReadFontFamily(JsonElement root, string key, string fallback)
        {
            if (!root.TryGetProperty(key, out var value) || value.ValueKind != JsonValueKind.String)
            {
                return fallback;
            }

            return (value.GetString() ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                "modern" => "\"Montserrat\", \"Segoe UI\", sans-serif",
                "serif" => "\"Merriweather\", Georgia, serif",
                "mono" => "\"JetBrains Mono\", Menlo, monospace",
                "classic" => "\"Times New Roman\", Times, serif",
                _ => fallback
            };
        }

        private sealed class ResumeTheme
        {
            public required string PrimaryColor { get; set; }
            public required string AccentColor { get; set; }
            public required string TextColor { get; set; }
            public required string MutedTextColor { get; set; }
            public required string BackgroundColor { get; set; }
            public required string BodyFontFamily { get; set; }
            public required string HeadingFontFamily { get; set; }

            public static ResumeTheme Default(string templateKey)
                => templateKey switch
                {
                    "impressive" => new ResumeTheme
                    {
                        PrimaryColor = "#1f4fb2",
                        AccentColor = "#2f80ed",
                        TextColor = "#162034",
                        MutedTextColor = "#52607a",
                        BackgroundColor = "#ffffff",
                        BodyFontFamily = "\"Segoe UI\", \"Helvetica Neue\", Arial, sans-serif",
                        HeadingFontFamily = "\"Montserrat\", \"Segoe UI\", sans-serif"
                    },
                    "professional" => new ResumeTheme
                    {
                        PrimaryColor = "#1f2937",
                        AccentColor = "#334155",
                        TextColor = "#111827",
                        MutedTextColor = "#4b5563",
                        BackgroundColor = "#ffffff",
                        BodyFontFamily = "\"Segoe UI\", \"Helvetica Neue\", Arial, sans-serif",
                        HeadingFontFamily = "\"Merriweather\", Georgia, serif"
                    },
                    "harvard" => new ResumeTheme
                    {
                        PrimaryColor = "#7c4f2f",
                        AccentColor = "#9a3412",
                        TextColor = "#292524",
                        MutedTextColor = "#57534e",
                        BackgroundColor = "#fffdf8",
                        BodyFontFamily = "\"Garamond\", \"Times New Roman\", serif",
                        HeadingFontFamily = "\"Merriweather\", Georgia, serif"
                    },
                    _ => new ResumeTheme
                    {
                        PrimaryColor = "#183153",
                        AccentColor = "#2f80ed",
                        TextColor = "#1f2937",
                        MutedTextColor = "#5b667a",
                        BackgroundColor = "#ffffff",
                        BodyFontFamily = "\"Segoe UI\", \"Helvetica Neue\", Arial, sans-serif",
                        HeadingFontFamily = "\"Montserrat\", \"Segoe UI\", sans-serif"
                    }
                };
        }
    }
}
