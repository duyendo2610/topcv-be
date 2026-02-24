using System.Security.Cryptography;
using topCv.Application.Common;
using topCv.Domain.Entities.Auth;

namespace topCv.Infrastructure.Security
{
    public class PasswordHasherService : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public string Hash(User user, string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(
                password,
                SaltSize,
                Iterations,
                HashAlgorithmName.SHA256
            );
            var salt = algorithm.Salt;
            var key = algorithm.GetBytes(KeySize);

            var result = new byte[SaltSize + KeySize];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(key, 0, result, SaltSize, KeySize);

            return Convert.ToBase64String(result);
        }

        public bool Verify(User user, string password)
        {
            var hashBytes = Convert.FromBase64String(user.PasswordHash);

            var salt = new byte[SaltSize];
            var key = new byte[KeySize];

            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(hashBytes, SaltSize, key, 0, KeySize);

            using var algorithm = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256
            );

            var keyToCheck = algorithm.GetBytes(KeySize);

            return CryptographicOperations.FixedTimeEquals(key, keyToCheck);
        }
    }
}