using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.Common
{

    public interface IFileStorage
    {
        /// <summary>
        /// Save file stream and return (publicUrl, storedFileName).
        /// publicUrl: đường dẫn client truy cập (vd: /uploads/abc.pdf)
        /// storedFileName: tên file đã lưu trên disk (vd: abc.pdf)
        /// </summary>
        Task<(string publicUrl, string storedFileName)> SaveAsync(
            Stream content,
            string originalFileName,
            string contentType,
            CancellationToken ct);

        /// <summary>
        /// Delete by publicUrl (vd: /uploads/abc.pdf)
        /// </summary>
        Task DeleteAsync(string publicUrl, CancellationToken ct);
    }
}
