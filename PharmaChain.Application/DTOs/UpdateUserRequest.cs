
using System.Text.Json.Serialization;

namespace PharmaChain.Application.DTOs
{
    public class UpdateUserRequest
    {
        public string UserId { get; set; }
        public string ColumnName { get; set; }
        public string UpdatedValue { get; set; }
    }

    public class UpdateUserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string UpdatedValue { get; set; }
    }
}
