namespace PharmaChain.Application.DTOs
{
    public class SupplierRequest
    {
        public class CreateSupplierRequest
        {
            public string SupplierId { get; set; } = string.Empty;
            public string SupplierName { get; set; }
            public string ContactPerson { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string GSTIN { get; set; }
            public string DrugLicenseNumber { get; set; }
            public string BankAccountNumber { get; set; }
            public string IFSCCode { get; set; }
            public string BankBranchName { get; set; }
            public bool IsActive { get; set; }
            public string? CreatedBy { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class CommonResponse
        {
            public bool Success { get; set; }
            public string Messege { get; set; }
        }

        public class SupplierListResponse
        {
            public int TotalSuppliers { get; set; }
            public int ActiveSuppliers { get; set; }
            public int InactiveSuppliers { get; set; }
            public int SuspendedSuppliers { get; set; }

            public List<CreateSupplierRequest> Records { get; set; } = new();

            public bool Success { get; set; }
            public string Messege { get; set; }
        }

    }
}
