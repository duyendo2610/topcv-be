using Microsoft.Extensions.Options;
using topCv.Application.Common;

namespace topCv.Infrastructure.Repositories
{
    public sealed class LocalFileStorage : IFileStorage
    {
        private readonly FileStorageOptions _opt;

        public LocalFileStorage(IOptions<FileStorageOptions> opt)
        {
            _opt = opt.Value;
        }

        public async Task<(string publicUrl, string storedFileName)> SaveAsync(
            Stream content,
            string originalFileName,
            string contentType,
            CancellationToken ct)
        {
            Directory.CreateDirectory(_opt.RootPath);

            var ext = Path.GetExtension(originalFileName);
            if (string.IsNullOrWhiteSpace(ext)) ext = "";

            var storedFileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(_opt.RootPath, storedFileName);

            await using var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await content.CopyToAsync(fs, ct);

            var publicUrl = $"{_opt.PublicBaseUrl.TrimEnd('/')}/{storedFileName}";
            return (publicUrl, storedFileName);
        }

        public Task DeleteAsync(string publicUrl, CancellationToken ct)
        {
            // publicUrl: /uploads/<file>
            var stored = publicUrl.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (string.IsNullOrWhiteSpace(stored)) return Task.CompletedTask;

            var fullPath = Path.Combine(_opt.RootPath, stored);
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }
    }
}