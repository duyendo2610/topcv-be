using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Application.DTOs.Auth
{
    internal class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = default!;
    }
}
