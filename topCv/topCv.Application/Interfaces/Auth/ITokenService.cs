using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Auth;

namespace topCv.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        string CreateAccessToken(User user, out DateTime expiresAtUtc);
        string CreateRefreshToken();
    }
}
