using System;
using System.Collections.Generic;

namespace PharmaChain.Application.DTOs.Sales
{
    public class SalesDashboardResponse
    {
        public MetricsDto Metrics { get; set; } = new();
        public List<TrendDto> Trend { get; set; } = new();
        public List<SupplierDto> SupplierContribution { get; set; } = new();
        public List<TopMedicineDto> TopMedicines { get; set; } = new();
        public List<CategoryDto> CategoryRevenue { get; set; } = new();
        public List<GstDto> GstBreakdown { get; set; } = new();
        public List<ExpiryRiskDto> ExpiryRisk { get; set; } = new();
        public AuditDto Audit { get; set; } = new();
    }

    public class MetricsDto
    {
        public decimal TotalSales { get; set; }
        public int UnitsSold { get; set; }
        public decimal AvgInvoice { get; set; }
        public decimal GrossMargin { get; set; }
        public decimal GstCollected { get; set; }
        public int ExpiryRiskCount { get; set; }
    }

    public class TrendDto
    {
        public string Label { get; set; }
        public decimal Revenue { get; set; }
        public int Units { get; set; }
    }

    public class SupplierDto
    {
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopMedicineDto
    {
        public string MedicineId { get; set; }
        public string Name { get; set; }
        public decimal Revenue { get; set; }
    }

    public class CategoryDto
    {
        public string Category { get; set; }
        public decimal Revenue { get; set; }
    }

    public class ExpiryRiskDto
    {
        public string Quarter { get; set; }
        public int RiskCount { get; set; }
    }

    public class GstDto
    {
        public int Gst { get; set; }
        public decimal Amount { get; set; }
    }

    public class SalesFilterDto
    {
        public string? BranchId { get; set; }
        public string? UserId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Category { get; set; }
        public string? SupplierId { get; set; }
        public string? ViewType { get; set; }
        public string? Metric { get; set; }
    }

    public class AuditDto
    {
        public List<AuditAnomalyDto> Anomalies { get; set; } = new();
        public List<AuditTableRowDto> Table { get; set; } = new();
    }

    public class AuditAnomalyDto
    {
        public string Type { get; set; }
        public string Medicine { get; set; }
        public string BatchNumber { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
    }

    public class AuditTableRowDto
    {
        public string MedicineName { get; set; }
        public string Category { get; set; }
        public string BatchNumber { get; set; }
        public string Supplier { get; set; }

        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Revenue { get; set; }

        public decimal Gst { get; set; }
        public decimal GstAmount { get; set; }

        public decimal Margin { get; set; }

        public DateTime ExpDate { get; set; }
        public string Status { get; set; }
    }

}