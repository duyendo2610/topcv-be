using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Auth;

namespace topCv.Application.Common
{
    public interface IPasswordHasher
    {
        string Hash(User user, string password);
        bool Verify(User user, string password);
    }
}
