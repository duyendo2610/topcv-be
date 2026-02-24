using topCv.Domain.Entities.Auth;

namespace topCv.Application.Common
{
    public interface IPasswordHasher
    {
        string Hash(User user, string password);
        bool Verify(User user, string password);
    }
}