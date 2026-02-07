using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Auth;
using topCv.Domain.Enums;

namespace topCv.Domain.Entities.Obj
{
    public class JobApplication
    {
        public Guid Id { get; set; }

        public Guid JobId { get; set; }
        public Guid CandidateUserId { get; set; }

        public Guid? ResumeId { get; set; }
        public Guid? ResumeFileId { get; set; }

        public string? CoverLetter { get; set; }
        public ApplicationStatus Status { get; set; }

        public DateTime AppliedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Job Job { get; set; } = default!;
        public User CandidateUser { get; set; } = default!;
        public Resume? Resume { get; set; }
        public ResumeFile? ResumeFile { get; set; }
    }
}
