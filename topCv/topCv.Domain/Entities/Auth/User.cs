using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace topCv.Domain.Entities.Auth
{
    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; }       
        public string PasswordHash { get; set; }  

        public string Role { get; set; }           

        public bool IsActive { get; set; }        
        public DateTime CreatedAtUtc { get; set; }

        // Navigation
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
