using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Infrastructure.Repositories
{
    public sealed class FileStorageOptions
    {
        public string RootPath { get; set; } = "wwwroot/uploads";
        public string PublicBaseUrl { get; set; } = "/uploads";
    }
}
