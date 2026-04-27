using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmaChain.Application.DTOs.Sales;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.Models;

namespace PharmaChain.Infrastructure
{
    public class ReportsService : IReportsService
    {
        private readonly IPharmaChainDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly IJwtService _jwtService;
        private readonly ILogService _logService;
        private readonly IMedicineService _medicineService;
        private readonly ISupplierService _supplierService;
        private readonly IBatchService _batchService;
        private readonly IPurchaseStockService _purchaseStockService;
        private readonly IStockTrackingService _stockTrackingService;

        public ReportsService(IPharmaChainDbContext context,
            IHttpContextAccessor httpContext,
            IAuthService authService, IOtpService otpService,
            IUserService userService, IRoleService roleService,
            IPermissionService permissionService,
            IJwtService jwtService,
            ILogService logService,
            IMedicineService medicineService,
            ISupplierService supplierService,
            IBatchService batchService,
            IPurchaseStockService purchaseStockService,
            IStockTrackingService stockTrackingService
            )
        {
            _context = context;
            _httpContext = httpContext;
            _authService = authService;
            _otpService = otpService;
            _userService = userService;
            _roleService = roleService;
            _permissionService = permissionService;
            _jwtService = jwtService;
            _logService = logService;
            _medicineService = medicineService;
            _supplierService = supplierService;
            _batchService = batchService;
            _purchaseStockService = purchaseStockService;
            _stockTrackingService = stockTrackingService;
        }

        public async Task<SalesDashboardResponse> GetSalesDashboardAsync(SalesFilterDto filter)
        {
            try
            {
              
                if (filter == null)
                    throw new ArgumentException("Filter cannot be null");

                if (string.IsNullOrEmpty(filter.UserId))
                    throw new ArgumentException("UserId is required");

                var user = await _context.Users
                    .AsNoTracking()
                    .Include(x => x.Role)
                    .FirstOrDefaultAsync(x => x.UserId == filter.UserId);

                if (user == null)
                    throw new KeyNotFoundException("User not found");

                bool isSuperAdmin =
                    string.Equals(user.Role?.RoleName, "SuperAdmin", StringComparison.OrdinalIgnoreCase);

                if (!isSuperAdmin)
                    filter.BranchId = user.BranchId;

                var invoicesQuery = _context.Invoices
                    .AsNoTracking()
                    .AsQueryable();

                if (!string.IsNullOrEmpty(filter.BranchId))
                    invoicesQuery = invoicesQuery.Where(x => x.BranchId == filter.BranchId);

                if (filter.DateFrom.HasValue)
                    invoicesQuery = invoicesQuery.Where(x => x.InvoiceDate >= filter.DateFrom.Value);

                if (filter.DateTo.HasValue)
                    invoicesQuery = invoicesQuery.Where(x => x.InvoiceDate <= filter.DateTo.Value);

                var itemsQuery =
                    from i in _context.InvoiceItems.AsNoTracking()
                    join inv in invoicesQuery
                        on i.InvoiceId equals inv.InvoiceId
                    select i;

                var medicinesQuery = _context.Medicines.AsNoTracking();
                var batchQuery = _context.MedicineBatches.AsNoTracking();

                var metricsTask = GetMetrics(invoicesQuery, itemsQuery);
                var trendTask = GetTrend(invoicesQuery);
                var categoryTask = GetCategory(itemsQuery, medicinesQuery);
                var topMedicinesTask = GetTopMedicines(itemsQuery, medicinesQuery);
                var supplierTask = GetSupplier(batchQuery);
                var gstTask = GetGst(invoicesQuery);
                var expiryTask = GetExpiry(batchQuery);
                var auditTask = GetAudit(itemsQuery, medicinesQuery);

                await Task.WhenAll(
                    metricsTask,
                    trendTask,
                    categoryTask,
                    topMedicinesTask,
                    supplierTask,
                    gstTask,
                    expiryTask,
                    auditTask
                );

                return new SalesDashboardResponse
                {
                    Metrics = await metricsTask,
                    Trend = await trendTask,
                    CategoryRevenue = await categoryTask,
                    TopMedicines = await topMedicinesTask,
                    SupplierContribution = await supplierTask,
                    GstBreakdown = await gstTask,
                    ExpiryRisk = await expiryTask,
                    Audit = await auditTask
                };
            }
            catch (ArgumentException ex)
            {
                throw;
            }
            catch (KeyNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load sales dashboard data", ex);
            }
        }

        private async Task<MetricsDto> GetMetrics(IQueryable<Invoice> invoices, IQueryable<InvoiceItem> items)
        {
            var totalSales = await invoices.SumAsync(x => (decimal?)x.TotalAmount) ?? 0;
            var gst = await invoices.SumAsync(x => (decimal?)x.TaxAmount) ?? 0;

            var count = await invoices.CountAsync();
            var avg = count == 0 ? 0 : totalSales / count;

            var units = await items.SumAsync(x => (int?)x.Quantity) ?? 0;

            return new MetricsDto
            {
                TotalSales = totalSales,
                GstCollected = gst,
                AvgInvoice = avg,
                UnitsSold = units,
                GrossMargin = 0,
                ExpiryRiskCount = 0
            };
        }

        private async Task<List<TrendDto>> GetTrend(IQueryable<Invoice> invoices)
        {
            return await invoices
                .GroupBy(x => x.InvoiceDate.Month)
                .Select(g => new TrendDto
                {
                    Label = "M" + g.Key,
                    Revenue = g.Sum(x => x.TotalAmount),
                    Units = 0
                })
                .ToListAsync();
        }

        private async Task<List<CategoryDto>> GetCategory(
    IQueryable<InvoiceItem> items,
    IQueryable<Medicine> medicines)
        {
            return await (
                from i in items
                join m in medicines on i.MedicineId equals m.MedicineId
                group new { i, m } by m.Category into g
                select new CategoryDto
                {
                    Category = g.Key,
                    Revenue = g.Sum(x => x.i.TotalPrice)
                }
            ).ToListAsync();
        }


        private async Task<List<TopMedicineDto>> GetTopMedicines(
    IQueryable<InvoiceItem> items,
    IQueryable<Medicine> medicines)
        {
            return await (
                from i in items
                join m in medicines on i.MedicineId equals m.MedicineId
                group new { i, m } by new { i.MedicineId, m.MedicineName } into g
                select new TopMedicineDto
                {
                    MedicineId = g.Key.MedicineId,
                    Name = g.Key.MedicineName,
                    Revenue = g.Sum(x => x.i.TotalPrice)
                }
            )
            .OrderByDescending(x => x.Revenue)
            .Take(10)
            .ToListAsync();
        }


        private async Task<List<SupplierDto>> GetSupplier(IQueryable<MedicineBatch> batch)
        {
            return await batch
                .GroupBy(x => x.SupplierId)
                .Select(g => new SupplierDto
                {
                    SupplierId = g.Key,
                    SupplierName = g.Key,
                    Revenue = g.Sum(x => x.TotalStockReceived * x.UnitSellingPrice)
                })
                .ToListAsync();
        }


        private async Task<List<GstDto>> GetGst(IQueryable<Invoice> invoices)
        {
            return await invoices
                .GroupBy(x => x.TaxAmount)
                .Select(g => new GstDto
                {
                    Gst = (int)g.Key,
                    Amount = g.Sum(x => x.TaxAmount)
                })
                .ToListAsync();
        }


        private async Task<List<ExpiryRiskDto>> GetExpiry(IQueryable<MedicineBatch> batch)
        {
            var threshold = DateOnly.FromDateTime(DateTime.Now.AddDays(90));

            return await batch
                .Where(x => x.ExpDate <= threshold)
                .GroupBy(x => x.ExpDate.Month)
                .Select(g => new ExpiryRiskDto
                {
                    Quarter = "Q" + ((g.Key - 1) / 3 + 1),
                    RiskCount = g.Count()
                })
                .ToListAsync();
        }

        private async Task<AuditDto> GetAudit(
    IQueryable<InvoiceItem> items,
    IQueryable<Medicine> medicines)
        {
            var table = await (
                from i in items
                join m in medicines on i.MedicineId equals m.MedicineId
                select new AuditTableRowDto
                {
                    MedicineName = m.MedicineName,
                    Category = m.Category,
                    BatchNumber = i.BatchId,
                    Supplier = "",

                    Units = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Revenue = i.TotalPrice,

                    Gst = i.GstPercentage,
                    GstAmount = (i.TotalPrice * i.GstPercentage / 100),

                    Margin = 0,
                    ExpDate = DateTime.Now.AddMonths(6),
                    Status = "OK"
                }
            ).ToListAsync();

            return new AuditDto
            {
                Table = table,
                Anomalies = new List<AuditAnomalyDto>()
            };
        }


        public async Task<List<TrendDto>> GetSalesTrendAsync(SalesFilterDto filter)
        {
            var invoices = ApplyFilters(filter);
            return await GetTrend(invoices);
        }

        public async Task<List<TopMedicineDto>> GetTopMedicinesAsync(SalesFilterDto filter)
        {
            var items = ApplyItems(filter);
            var medicines = _context.Medicines.AsNoTracking();
            return await GetTopMedicines(items, medicines);
        }

        public async Task<List<CategoryDto>> GetCategoryRevenueAsync(SalesFilterDto filter)
        {
            var items = ApplyItems(filter);
            var medicines = _context.Medicines.AsNoTracking();
            return await GetCategory(items, medicines);
        }

        public async Task<List<SupplierDto>> GetSupplierContributionAsync(SalesFilterDto filter)
        {
            var batch = ApplyBatch(filter);
            return await GetSupplier(batch);
        }

        public async Task<List<GstDto>> GetGstBreakdownAsync(SalesFilterDto filter)
        {
            var invoices = ApplyFilters(filter);
            return await GetGst(invoices);
        }

        public async Task<MetricsDto> GetSalesMetricsAsync(SalesFilterDto filter)
        {
            var invoices = ApplyFilters(filter);
            var items = ApplyItems(filter);
            return await GetMetrics(invoices, items);
        }

        public async Task<List<ExpiryRiskDto>> GetExpiryRiskReportAsync(SalesFilterDto filter)
        {
            var batch = ApplyBatch(filter);
            return await GetExpiry(batch);
        }

        public async Task<AuditDto> GetAuditReportAsync(SalesFilterDto filter)
        {
            var items = ApplyItems(filter);
            var medicines = _context.Medicines.AsNoTracking();
            return await GetAudit(items, medicines);
        }


        private IQueryable<Invoice> ApplyFilters(SalesFilterDto filter)
        {
            var q = _context.Invoices.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(filter.BranchId))
                q = q.Where(x => x.BranchId == filter.BranchId);

            if (filter.DateFrom.HasValue)
                q = q.Where(x => x.InvoiceDate >= filter.DateFrom);

            if (filter.DateTo.HasValue)
                q = q.Where(x => x.InvoiceDate <= filter.DateTo);

            return q;
        }

        private IQueryable<InvoiceItem> ApplyItems(SalesFilterDto filter)
        {
            var invoiceIds = ApplyFilters(filter).Select(x => x.InvoiceId);
            return _context.InvoiceItems.Where(x => invoiceIds.Contains(x.InvoiceId));
        }

        private IQueryable<MedicineBatch> ApplyBatch(SalesFilterDto filter)
        {
            var q = _context.MedicineBatches.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(filter.SupplierId))
                q = q.Where(x => x.SupplierId == filter.SupplierId);

            return q;
        }


        public async Task<byte[]> ExportSalesReportCsvAsync(SalesFilterDto filter)
        {
            var invoices = ApplyFilters(filter);
            var items = ApplyItems(filter);
            var medicines = _context.Medicines.AsNoTracking();

            var data = await (
                from i in items
                join m in medicines on i.MedicineId equals m.MedicineId
                select new
                {
                    m.MedicineName,
                    i.Quantity,
                    i.UnitPrice,
                    i.TotalPrice,
                    i.GstPercentage
                }
            ).ToListAsync();

            var csv = new System.Text.StringBuilder();

            csv.AppendLine("Medicine,Quantity,UnitPrice,TotalPrice,GST");

            foreach (var row in data)
            {
                csv.AppendLine($"{row.MedicineName},{row.Quantity},{row.UnitPrice},{row.TotalPrice},{row.GstPercentage}");
            }

            return System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<byte[]> ExportSalesReportExcelAsync(SalesFilterDto filter)
        {
            var csvBytes = await ExportSalesReportCsvAsync(filter);

            return csvBytes;
        }
    }
}
