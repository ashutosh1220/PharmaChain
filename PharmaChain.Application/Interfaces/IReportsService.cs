using PharmaChain.Application.DTOs.Sales;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PharmaChain.Application.Interfaces
{
    public interface IReportsService
    {
        Task<SalesDashboardResponse> GetSalesDashboardAsync(SalesFilterDto filter);

        Task<List<TrendDto>> GetSalesTrendAsync(SalesFilterDto filter);

        Task<List<TopMedicineDto>> GetTopMedicinesAsync(SalesFilterDto filter);

        Task<List<CategoryDto>> GetCategoryRevenueAsync(SalesFilterDto filter);

        Task<List<SupplierDto>> GetSupplierContributionAsync(SalesFilterDto filter);

        Task<List<GstDto>> GetGstBreakdownAsync(SalesFilterDto filter);

        Task<MetricsDto> GetSalesMetricsAsync(SalesFilterDto filter);

        Task<List<ExpiryRiskDto>> GetExpiryRiskReportAsync(SalesFilterDto filter);

        Task<AuditDto> GetAuditReportAsync(SalesFilterDto filter);

        Task<byte[]> ExportSalesReportCsvAsync(SalesFilterDto filter);

        Task<byte[]> ExportSalesReportExcelAsync(SalesFilterDto filter);
    }
}