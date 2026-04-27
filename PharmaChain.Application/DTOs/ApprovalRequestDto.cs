public class ApprovalRequestDto
{
    public string AssignedStockId { get; set; } = null!;
    public string ApprovalStatus { get; set; } = "APPROVED";
    public string? Remark { get; set; }
    public string FulfillmentType { get; set; } = null!;
    public string ApprovedBy { get; set; } = null!;

    public List<ApprovedMedicineDto> Medicines { get; set; } = new();
}

public class ApprovedMedicineDto
{
    public string MedicineId { get; set; } = null!;
    public int QtyApproved { get; set; }
}

public class RejectStockRequestDto
{
    public string AssignedStockId { get; set; } = null!;
    public string Remark { get; set; } = null!;
}