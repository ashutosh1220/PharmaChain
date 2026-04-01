namespace PharmaChain.Application.DTOs
{
    public class GetUserRequest
    {
        // Search
        public string? Search { get; set; }

        // Filters
        public string? Role { get; set; }
        public string? Branch { get; set; }
        public string? Status { get; set; }

        // Sorting
        public string? SortField { get; set; }
        public string? SortOrder { get; set; } 

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
