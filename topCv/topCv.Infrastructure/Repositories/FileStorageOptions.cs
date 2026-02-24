namespace topCv.Infrastructure.Repositories
{
    public sealed class FileStorageOptions
    {
        public string RootPath { get; set; } = "wwwroot/uploads";
        public string PublicBaseUrl { get; set; } = "/uploads";
    }
}