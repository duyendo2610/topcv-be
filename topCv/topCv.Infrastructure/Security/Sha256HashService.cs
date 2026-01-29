using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.Common;

namespace topCv.Infrastructure.Security
{
    public class Sha256HashService : IHashService
    {
        public string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha.ComputeHash(bytes);

            return Convert.ToHexString(hashBytes); 
        }
    }
}
