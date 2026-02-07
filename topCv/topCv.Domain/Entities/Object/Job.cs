using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Auth;
using topCv.Domain.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace topCv.Domain.Entities.Obj
{
    public class Job
    {
        public Guid Id { get; set; }

        public Guid CompanyId { get; set; }
        public Guid CreatedByUserId { get; set; }

        public string Title { get; set; } = default!;
        public JobLevel Level { get; set; }
        public JobType JobType { get; set; }

        public string Description { get; set; } = default!;
        public string? Requirement { get; set; }
        public string? Benefit { get; set; }

        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? Currency { get; set; }

        public int? CityId { get; set; }
        public string? Address { get; set; }

        public int? ExpMin { get; set; }
        public int? ExpMax { get; set; }

        public DateTime? DeadlineAt { get; set; }
        public JobStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Company Company { get; set; } = default!;
        public User CreatedByUser { get; set; } = default!;
        public City? City { get; set; }

        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        public ICollection<JobCategory> JobCategories { get; set; } = new List<JobCategory>();

        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
        public ICollection<SavedJob> SavedByUsers { get; set; } = new List<SavedJob>();
    }

}
