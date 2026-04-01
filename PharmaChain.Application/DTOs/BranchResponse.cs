using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaChain.Application.DTOs
{
    public class BranchResponse
    {
        public string BranchId { get; set; } = null!;
        public string BranchName { get; set; } = null!;
        public string AddressLine1 { get; set; } = null!;
        public string City { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
