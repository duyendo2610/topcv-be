using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Mappings;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Services.Commons
{
    public sealed class ResumeService : IResumeService
    {
        private static readonly Regex SlugRegex = new("[^a-z0-9-]", RegexOptions.Compiled);
        private static readonly HashSet<string> SupportedTemplateKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "simple", "impressive", "professional", "harvard"
        };

        private readonly IAppDbContext _db;
        private readonly IFileStorage _storage;

        public ResumeService(IAppDbContext db, IFileStorage storage)
        {
            _db = db;
            _storage = storage;
        }

        public async Task<ResumeDetailResponse> CreateAsync(Guid userId, CreateResumeRequest req, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var (variant, themePreset) = await ResolveDesignSelectionAsync(
                req.TemplateVariantId,
                req.ThemePresetId,
                req.TemplateKey,
                null,
                null,
                null,
                ct);

            var resume = new Resume
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = NormalizeName(req.Name),
                TemplateKey = variant.TemplateKey,
                TemplateVariantId = variant.Id,
                ThemePresetId = themePreset?.Id,
                ThemeJson = NormalizeThemeJson(req.Theme),
                Visibility = req.Visibility,
                IsDefault = req.IsDefault,
                CreatedAt = now
            };

            var hasAnyResume = await _db.Resumes
                .AsNoTracking()
                .AnyAsync(x => x.UserId == userId, ct);

            if (!hasAnyResume)
            {
                resume.IsDefault = true;
            }

            if (resume.IsDefault)
            {
                await UnsetOtherDefaultAsync(userId, resume.Id, now, ct);
            }

            _db.Resumes.Add(resume);

            var sections = BuildSections(req.Sections, resume.Id, now);
            if (sections.Count > 0)
            {
                _db.ResumeSections.AddRange(sections);
                resume.Sections = sections;
            }

            resume.TemplateVariant = variant;
            resume.ThemePreset = themePreset;
            await _db.SaveChangesAsync(ct);
            return resume.ToDetailResponse();
        }

        public async Task<List<ResumeSummaryResponse>> GetMineAsync(Guid userId, CancellationToken ct)
        {
            var rows = await _db.Resumes
                .AsNoTracking()
                .Include(x => x.TemplateVariant)
                .Include(x => x.ThemePreset)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToListAsync(ct);

            return rows.Select(x => x.ToSummaryResponse()).ToList();
        }

        public async Task<ResumeDetailResponse> GetByIdAsync(Guid userId, Guid resumeId, CancellationToken ct)
        {
            var resume = await GetOwnedResumeAsync(userId, resumeId, asTracking: false, includeFiles: true, ct);
            return resume.ToDetailResponse();
        }

        public async Task<ResumeDetailResponse> UpdateAsync(Guid userId, Guid resumeId, UpdateResumeRequest req, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var resume = await GetOwnedResumeAsync(userId, resumeId, asTracking: true, includeFiles: true, ct);

            if (req.Name is not null)
                resume.Name = NormalizeName(req.Name);

            if (req.Visibility.HasValue)
                resume.Visibility = req.Visibility.Value;

            var (variant, themePreset) = await ResolveDesignSelectionAsync(
                req.TemplateVariantId,
                req.ThemePresetId,
                req.TemplateKey,
                resume.TemplateVariant,
                resume.ThemePreset,
                resume.TemplateKey,
                ct);

            resume.TemplateKey = variant.TemplateKey;
            resume.TemplateVariantId = variant.Id;
            resume.ThemePresetId = themePreset?.Id;
            resume.TemplateVariant = variant;
            resume.ThemePreset = themePreset;

            if (req.Theme.HasValue)
                resume.ThemeJson = NormalizeThemeJson(req.Theme);

            if (req.IsDefault.HasValue)
            {
                if (req.IsDefault.Value)
                {
                    await UnsetOtherDefaultAsync(userId, resume.Id, now, ct);
                    resume.IsDefault = true;
                }
                else if (resume.IsDefault)
                {
                    var fallback = await _db.Resumes
                        .Where(x => x.UserId == userId && x.Id != resume.Id)
                        .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                        .FirstOrDefaultAsync(ct);

                    if (fallback is null)
                        throw new InvalidOperationException("KhÃ´ng thá»ƒ bá» Ä‘Ã¡nh dáº¥u máº·c Ä‘á»‹nh cá»§a CV duy nháº¥t.");

                    fallback.IsDefault = true;
                    fallback.UpdatedAt = now;
                    resume.IsDefault = false;
                }
                else
                {
                    resume.IsDefault = false;
                }
            }

            if (req.Sections is not null)
            {
                _db.ResumeSections.RemoveRange(resume.Sections);
                resume.Sections.Clear();

                var sections = BuildSections(req.Sections, resume.Id, now);
                if (sections.Count > 0)
                {
                    _db.ResumeSections.AddRange(sections);
                    foreach (var section in sections)
                    {
                        resume.Sections.Add(section);
                    }
                }
            }

            resume.UpdatedAt = now;
            await _db.SaveChangesAsync(ct);
            return resume.ToDetailResponse();
        }

        public async Task DeleteAsync(Guid userId, Guid resumeId, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var resume = await _db.Resumes
                .FirstOrDefaultAsync(x => x.Id == resumeId && x.UserId == userId, ct)
                ?? throw new KeyNotFoundException("KhÃ´ng tÃ¬m tháº¥y CV.");

            if (resume.IsDefault)
            {
                var fallback = await _db.Resumes
                    .Where(x => x.UserId == userId && x.Id != resume.Id)
                    .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                    .FirstOrDefaultAsync(ct);

                if (fallback is not null)
                {
                    fallback.IsDefault = true;
                    fallback.UpdatedAt = now;
                }
            }

            _db.Resumes.Remove(resume);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<ResumePreviewResponse> PreviewAsync(Guid userId, Guid resumeId, CancellationToken ct)
        {
            var resume = await GetOwnedResumeAsync(userId, resumeId, asTracking: false, includeFiles: false, ct);
            var html = ResumeHtmlRenderer.Render(resume, resume.Sections.ToList(), resume.TemplateVariant, resume.ThemePreset);

            return new ResumePreviewResponse
            {
                ResumeId = resume.Id,
                ResumeName = resume.Name,
                TemplateKey = resume.TemplateKey,
                TemplateVariantKey = resume.TemplateVariant?.VariantKey,
                ThemePresetKey = resume.ThemePreset?.ThemeKey,
                Html = html,
                GeneratedAtUtc = DateTime.UtcNow
            };
        }

        public async Task<ResumePreviewResponse> PreviewForEmployerAsync(Guid applicationId, Guid employerUserId,
            CancellationToken ct)
        {
            var application = await _db.JobApplications
                                 .AsNoTracking()
                                 .Include(x => x.Job)
                                 .ThenInclude(x => x.Company)
                                 .FirstOrDefaultAsync(x => x.Id == applicationId, ct)
                             ?? throw new KeyNotFoundException("Không tìm thấy hồ sơ ứng tuyển.");

            if (application.Job?.Company == null)
                throw new KeyNotFoundException("Không tìm thấy công ty.");

            if (application.Job.Company.OwnerUserId != employerUserId)
                throw new UnauthorizedAccessException("Bạn không phải chủ sở hữu công ty.");

            if (application.ResumeId == null)
                throw new InvalidOperationException("Ứng viên chưa có CV để xem trước.");

            var resume = await _db.Resumes
                .AsNoTracking()
                .Include(x => x.Sections)
                .Include(x => x.TemplateVariant)
                .Include(x => x.ThemePreset)
                .FirstOrDefaultAsync(x => x.Id == application.ResumeId.Value, ct)
                ?? throw new KeyNotFoundException("Không tìm thấy CV.");

            var html = ResumeHtmlRenderer.Render(resume, resume.Sections.ToList(), resume.TemplateVariant, resume.ThemePreset);

            return new ResumePreviewResponse
            {
                ResumeId = resume.Id,
                ResumeName = resume.Name,
                TemplateKey = resume.TemplateKey,
                TemplateVariantKey = resume.TemplateVariant?.VariantKey,
                ThemePresetKey = resume.ThemePreset?.ThemeKey,
                Html = html,
                GeneratedAtUtc = DateTime.UtcNow
            };
        }

        public async Task<ResumeFileResponse> ExportHtmlAsync(Guid userId, Guid resumeId, CancellationToken ct)
        {
            var preview = await PreviewAsync(userId, resumeId, ct);
            var bytes = Encoding.UTF8.GetBytes(preview.Html);
            await using var stream = new MemoryStream(bytes);

            var fileName = $"{ToSlug(preview.ResumeName)}-{DateTime.UtcNow:yyyyMMddHHmmss}.html";
            var (fileUrl, _) = await _storage.SaveAsync(stream, fileName, "text/html", ct);

            var fileEntity = new ResumeFile
            {
                Id = Guid.NewGuid(),
                ResumeId = resumeId,
                FileUrl = fileUrl,
                FileName = fileName,
                MimeType = "text/html",
                FileSize = bytes.LongLength,
                UploadedAt = DateTime.UtcNow
            };

            _db.ResumeFiles.Add(fileEntity);
            await _db.SaveChangesAsync(ct);

            return fileEntity.ToResponse();
        }

        private async Task<Resume> GetOwnedResumeAsync(
            Guid userId,
            Guid resumeId,
            bool asTracking,
            bool includeFiles,
            CancellationToken ct)
        {
            IQueryable<Resume> query = _db.Resumes
                .Where(x => x.Id == resumeId && x.UserId == userId)
                .Include(x => x.Sections)
                .Include(x => x.TemplateVariant)
                .Include(x => x.ThemePreset);

            if (includeFiles)
            {
                query = query.Include(x => x.Files);
            }

            if (!asTracking)
            {
                query = query.AsNoTracking();
            }

            var resume = await query.FirstOrDefaultAsync(ct);
            return resume ?? throw new KeyNotFoundException("KhÃ´ng tÃ¬m tháº¥y CV.");
        }

        private async Task UnsetOtherDefaultAsync(Guid userId, Guid currentResumeId, DateTime now, CancellationToken ct)
        {
            var defaults = await _db.Resumes
                .Where(x => x.UserId == userId && x.Id != currentResumeId && x.IsDefault)
                .ToListAsync(ct);

            foreach (var item in defaults)
            {
                item.IsDefault = false;
                item.UpdatedAt = now;
            }
        }

        private async Task<(ResumeTemplateVariant variant, ResumeThemePreset? themePreset)> ResolveDesignSelectionAsync(
            Guid? templateVariantId,
            Guid? themePresetId,
            string? templateKeyInput,
            ResumeTemplateVariant? currentVariant,
            ResumeThemePreset? currentThemePreset,
            string? fallbackTemplateKey,
            CancellationToken ct)
        {
            ResumeTemplateVariant variant;
            var normalizedTemplateKey = NormalizeTemplateKey(templateKeyInput);

            if (templateVariantId.HasValue)
            {
                if (currentVariant is not null && currentVariant.Id == templateVariantId.Value)
                {
                    variant = currentVariant;
                }
                else
                {
                    variant = await _db.ResumeTemplateVariants
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == templateVariantId.Value && x.IsActive, ct)
                        ?? throw new InvalidOperationException("KhÃ´ng tÃ¬m tháº¥y biáº¿n thá»ƒ máº«u CV.");
                }

                if (normalizedTemplateKey is not null &&
                    !string.Equals(variant.TemplateKey, normalizedTemplateKey, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Biáº¿n thá»ƒ máº«u khÃ´ng thuá»™c TemplateKey Ä‘Ã£ chá»n.");
                }
            }
            else if (normalizedTemplateKey is not null)
            {
                if (currentVariant is not null &&
                    currentVariant.IsActive &&
                    string.Equals(currentVariant.TemplateKey, normalizedTemplateKey, StringComparison.OrdinalIgnoreCase))
                {
                    variant = currentVariant;
                }
                else
                {
                    variant = await GetDefaultVariantByTemplateKeyAsync(normalizedTemplateKey, ct);
                }
            }
            else if (currentVariant is not null && currentVariant.IsActive)
            {
                variant = currentVariant;
            }
            else
            {
                variant = await GetDefaultVariantByTemplateKeyAsync(
                    NormalizeTemplateKey(fallbackTemplateKey, strict: false) ?? "simple",
                    ct);
            }

            ResumeThemePreset? themePreset;
            if (themePresetId.HasValue)
            {
                if (currentThemePreset is not null && currentThemePreset.Id == themePresetId.Value)
                {
                    themePreset = currentThemePreset;
                }
                else
                {
                    themePreset = await _db.ResumeThemePresets
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == themePresetId.Value && x.IsActive, ct)
                        ?? throw new InvalidOperationException("KhÃ´ng tÃ¬m tháº¥y bá»™ giao diá»‡n.");
                }

                var allowed = await _db.ResumeTemplateVariantThemes
                    .AsNoTracking()
                    .AnyAsync(x => x.VariantId == variant.Id && x.ThemePresetId == themePreset.Id, ct);

                if (!allowed)
                    throw new InvalidOperationException("Bá»™ giao diá»‡n khÃ´ng Ä‘Æ°á»£c há»— trá»£ bá»Ÿi biáº¿n thá»ƒ máº«u Ä‘Ã£ chá»n.");
            }
            else if (currentThemePreset is not null && currentThemePreset.IsActive)
            {
                var allowed = await _db.ResumeTemplateVariantThemes
                    .AsNoTracking()
                    .AnyAsync(x => x.VariantId == variant.Id && x.ThemePresetId == currentThemePreset.Id, ct);
                themePreset = allowed ? currentThemePreset : await GetDefaultThemeForVariantAsync(variant.Id, ct);
            }
            else
            {
                themePreset = await GetDefaultThemeForVariantAsync(variant.Id, ct);
            }

            return (variant, themePreset);
        }

        private async Task<ResumeTemplateVariant> GetDefaultVariantByTemplateKeyAsync(string templateKey, CancellationToken ct)
        {
            var variant = await _db.ResumeTemplateVariants
                .AsNoTracking()
                .Where(x => x.TemplateKey == templateKey && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .FirstOrDefaultAsync(ct);

            return variant ?? throw new InvalidOperationException($"KhÃ´ng tÃ¬m tháº¥y biáº¿n thá»ƒ Ä‘ang hoáº¡t Ä‘á»™ng cho templateKey '{templateKey}'.");
        }

        private async Task<ResumeThemePreset?> GetDefaultThemeForVariantAsync(Guid variantId, CancellationToken ct)
        {
            return await _db.ResumeTemplateVariantThemes
                .AsNoTracking()
                .Where(x => x.VariantId == variantId && x.ThemePreset.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ThemePreset.SortOrder)
                .Select(x => x.ThemePreset)
                .FirstOrDefaultAsync(ct);
        }

        private static List<ResumeSection> BuildSections(IEnumerable<ResumeSectionInput>? sections, Guid resumeId, DateTime now)
        {
            var result = new List<ResumeSection>();
            if (sections is null) return result;

            foreach (var input in sections)
            {
                var content = NormalizeSectionContent(input.Content);
                var title = NormalizeSectionTitle(input.Title);
                result.Add(new ResumeSection
                {
                    Id = Guid.NewGuid(),
                    ResumeId = resumeId,
                    Type = input.Type,
                    Title = title,
                    SortOrder = input.SortOrder,
                    ContentJson = content,
                    CreatedAt = now
                });
            }

            return result;
        }

        private static string NormalizeName(string name)
        {
            var trimmed = name.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                throw new ArgumentException("TÃªn khÃ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng.");

            if (trimmed.Length > 200)
                throw new ArgumentException("Äá»™ dÃ i tÃªn tá»‘i Ä‘a 200 kÃ½ tá»±.");

            return trimmed;
        }

        private static string? NormalizeTemplateKey(string? key, bool strict = true)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            var normalized = key.Trim().ToLowerInvariant();
            if (!SupportedTemplateKeys.Contains(normalized))
            {
                if (strict) throw new ArgumentException("TemplateKey khÃ´ng Ä‘Æ°á»£c há»— trá»£.");
                return null;
            }

            return normalized;
        }

        private static string? NormalizeSectionTitle(string? title)
        {
            if (string.IsNullOrWhiteSpace(title)) return null;

            var trimmed = title.Trim();
            if (trimmed.Length > 200)
                throw new ArgumentException("Äá»™ dÃ i tiÃªu Ä‘á» má»¥c tá»‘i Ä‘a 200 kÃ½ tá»±.");

            return trimmed;
        }

        private static string NormalizeSectionContent(JsonElement content)
        {
            if (content.ValueKind == JsonValueKind.Undefined)
                throw new ArgumentException("Ná»™i dung má»¥c lÃ  báº¯t buá»™c.");

            return content.GetRawText();
        }

        private static string NormalizeThemeJson(JsonElement? theme)
        {
            if (!theme.HasValue)
                return "{}";

            var value = theme.Value;
            if (value.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
                return "{}";

            if (value.ValueKind != JsonValueKind.Object)
                throw new ArgumentException("Theme pháº£i lÃ  má»™t Ä‘á»‘i tÆ°á»£ng JSON.");

            return value.GetRawText();
        }

        private static string ToSlug(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return "resume";

            var raw = source.Trim().ToLowerInvariant()
                .Replace(' ', '-')
                .Replace('_', '-');

            raw = SlugRegex.Replace(raw, "-");
            raw = Regex.Replace(raw, "-{2,}", "-").Trim('-');
            return string.IsNullOrWhiteSpace(raw) ? "resume" : raw;
        }
    }
}

