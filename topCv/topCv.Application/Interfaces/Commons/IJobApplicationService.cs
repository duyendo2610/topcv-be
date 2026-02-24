using topCv.Application.DTOs.Commons;

namespace topCv.Application.Interfaces.Commons
{
    public interface IJobApplicationService
    {
        Task<JobApplicationResponse> ApplyAsync(CreateJobApplicationRequest req, Guid userId, CancellationToken ct);

        Task<List<JobApplicationResponse>> GetMyApplicationsAsync(Guid userId, CancellationToken ct);

        // Employer xem ứng viên theo Job
        Task<List<JobApplicationResponse>> GetByJobAsync(Guid jobId, Guid employerUserId, CancellationToken ct);

        // Employer cập nhật trạng thái
        Task<JobApplicationResponse> UpdateStatusAsync(Guid applicationId, UpdateJobApplicationStatusRequest req,
            Guid employerUserId, CancellationToken ct);

        // Candidate sửa nội dung đơn (optional nhưng clean)
        Task<JobApplicationResponse> UpdateMyApplicationAsync(Guid applicationId, UpdateJobApplicationRequest req,
            Guid userId, CancellationToken ct);
    }
}