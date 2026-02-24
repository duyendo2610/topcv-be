using Microsoft.EntityFrameworkCore;
using topCv.Application.Common;
using topCv.Application.DTOs.Commons;
using topCv.Application.Interfaces.Commons;
using topCv.Application.Mappings;
using topCv.Domain.Entities.Commons;

namespace topCv.Application.Services.Commons
{
    public sealed class ResumeFileService : IResumeFileService
    {
        private readonly IAppDbContext _db;
        private readonly IFileStorage _storage;

        public ResumeFileService(IAppDbContext db, IFileStorage storage)
        {
            _db = db;
            _storage = storage;
        }

        public async Task<ResumeFileResponse> UploadAsync(
            Guid userId,
            Guid resumeId,
            Stream content,
            string fileName,
            string contentType,
            long fileSize,
            CancellationToken ct)
        {
            if (fileSize <= 0) throw new ArgumentException("File is empty.");

            // 1) Check resume exists + belongs to current user
            var resume = await _db.Resumes
                             .AsNoTracking()
                             .FirstOrDefaultAsync(x => x.Id == resumeId, ct)
                         ?? throw new KeyNotFoundException("Resume not found.");

            if (resume.UserId != userId)
                throw new UnauthorizedAccessException("Not your resume.");

            // 2) Validate mime type (basic)
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            };

            if (!allowed.Contains(contentType))
                throw new InvalidOperationException("Only PDF/DOC/DOCX allowed.");

            // 3) Save physical file
            var (fileUrl, storedName) = await _storage.SaveAsync(content, fileName, contentType, ct);

            // 4) Create ResumeFile entity (NO UserId)
            var entity = new ResumeFile
            {
                Id = Guid.NewGuid(),
                ResumeId = resumeId,
                FileUrl = fileUrl,
                FileName = fileName,
                MimeType = contentType,
                FileSize = fileSize,
                UploadedAt = DateTime.UtcNow
            };

            _db.ResumeFiles.Add(entity);
            await _db.SaveChangesAsync(ct);

            return entity.ToResponse();
        }

        public async Task<List<ResumeFileResponse>> GetByResumeAsync(Guid userId, Guid resumeId, CancellationToken ct)
        {
            // check ownership
            var resume = await _db.Resumes
                             .AsNoTracking()
                             .FirstOrDefaultAsync(x => x.Id == resumeId, ct)
                         ?? throw new KeyNotFoundException("Resume not found.");

            if (resume.UserId != userId)
                throw new UnauthorizedAccessException("Not your resume.");

            var items = await _db.ResumeFiles
                .AsNoTracking()
                .Where(x => x.ResumeId == resumeId)
                .OrderByDescending(x => x.UploadedAt)
                .ToListAsync(ct);

            return items.Select(x => x.ToResponse()).ToList();
        }

        public async Task<ResumeFileResponse> GetByIdAsync(Guid userId, Guid id, CancellationToken ct)
        {
            // Need join to Resumes to check owner
            var entity = await _db.ResumeFiles
                             .AsNoTracking()
                             .FirstOrDefaultAsync(x => x.Id == id, ct)
                         ?? throw new KeyNotFoundException("Resume file not found.");

            var resume = await _db.Resumes
                             .AsNoTracking()
                             .FirstOrDefaultAsync(x => x.Id == entity.ResumeId, ct)
                         ?? throw new KeyNotFoundException("Resume not found.");

            if (resume.UserId != userId)
                throw new UnauthorizedAccessException("Not your resume file.");

            return entity.ToResponse();
        }

        public async Task DeleteAsync(Guid userId, Guid id, CancellationToken ct)
        {
            var entity = await _db.ResumeFiles
                             .FirstOrDefaultAsync(x => x.Id == id, ct)
                         ?? throw new KeyNotFoundException("Resume file not found.");

            var resume = await _db.Resumes
                             .AsNoTracking()
                             .FirstOrDefaultAsync(x => x.Id == entity.ResumeId, ct)
                         ?? throw new KeyNotFoundException("Resume not found.");

            if (resume.UserId != userId)
                throw new UnauthorizedAccessException("Not your resume file.");

            await _storage.DeleteAsync(entity.FileUrl, ct);

            _db.ResumeFiles.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}