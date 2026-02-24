using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Application.DTOs.Obj;
using topCv.Domain.Entities.Obj;

namespace topCv.Application.Mappings
{
    public static class JobMapping
    {
        public static Job ToJob(this CreateJobRequest req, Guid createdByUserId)
            => new()
            {
                Id = Guid.NewGuid(),
                CompanyId = req.CompanyId,
                CreatedByUserId = createdByUserId,

                Title = req.Title.Trim(),
                Level = req.Level,
                JobType = req.JobType,

                Description = req.Description,
                Requirement = req.Requirement,
                Benefit = req.Benefit,

                SalaryMin = req.SalaryMin,
                SalaryMax = req.SalaryMax,
                Currency = req.Currency,

                CityId = req.CityId,
                Address = req.Address,

                ExpMin = req.ExpMin,
                ExpMax = req.ExpMax,

                DeadlineAt = req.DeadlineAtUtc, // nếu entity tên DeadlineAt
                Status = 0, // Draft
            };

        public static void ApplyTo(this UpdateJobRequest req, Job job)
        {
            job.Title = req.Title.Trim();
            job.Level = req.Level;
            job.JobType = req.JobType;

            job.Description = req.Description;
            job.Requirement = req.Requirement;
            job.Benefit = req.Benefit;

            job.SalaryMin = req.SalaryMin;
            job.SalaryMax = req.SalaryMax;
            job.Currency = req.Currency;

            job.CityId = req.CityId;
            job.Address = req.Address;

            job.ExpMin = req.ExpMin;
            job.ExpMax = req.ExpMax;

            job.DeadlineAt = req.DeadlineAtUtc;
        }

        public static JobResponse ToResponse(this Job job)
            => new()
            {
                Id = job.Id,
                CompanyId = job.CompanyId,
                CompanyName = job.Company?.Name ?? "",
                CompanyLogoUrl = job.Company?.LogoUrl,
                CreatedByUserId = job.CreatedByUserId,

                Title = job.Title,
                Level = job.Level,
                JobType = job.JobType,

                Description = job.Description,
                Requirement = job.Requirement,
                Benefit = job.Benefit,

                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                Currency = job.Currency,

                CityId = job.CityId,
                CityName = job.Province?.Name,
                Address = job.Address,

                ExpMin = job.ExpMin,
                ExpMax = job.ExpMax,

                DeadlineAtUtc = job.DeadlineAt,
                Status = job.Status,

                SkillIds = job.JobSkills?.Select(x => x.SkillId).ToList() ?? [],
                CategoryIds = job.JobCategories?.Select(x => x.CategoryId).ToList() ?? [],
            };
    }
}
