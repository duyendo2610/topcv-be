using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Auth;
using topCv.Domain.Enums;

namespace topCv.Domain.Entities.Obj
{
    public class CandidateProfile
    {
        public Guid UserId { get; set; } // PK + FK

        public string? Title { get; set; }
        public DateTime? Dob { get; set; }
        public Gender? Gender { get; set; }

        public int? CityId { get; set; }
        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }
        public string? About { get; set; }
        public int? YearsOfExperience { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public User User { get; set; } = default!;
        public City? City { get; set; }
    }
}
