namespace topCv.Application.DTOs.Commons
{
    public sealed class EmployerCompanyResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
    }
}