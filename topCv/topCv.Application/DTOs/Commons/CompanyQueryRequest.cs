namespace topCv.Application.DTOs.Commons
{
    public sealed class CompanyQueryRequest
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
