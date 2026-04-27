using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaChain.Application.Interfaces;
using System.Security.Claims;

namespace PharmaChain.Web.Controllers
{

    public class StockTransactionsController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IBranchService _branchService;
        private readonly IPharmaChainDbContext _context;
        public StockTransactionsController(IUserService userService,
            IPermissionService permissionService,
            IBranchService branchService,
            IPharmaChainDbContext context)
        {
            _userService = userService;
            _permissionService = permissionService;
            _branchService = branchService;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        [Route("Dashboard/StockTransaction/PurchaseInvoice")]
        public async Task<IActionResult> PurchaseInvoiceForm()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var permissions = await _permissionService.GetAllPermissionsForRolesAsync(role);

            var branches = await _context.Branches
                .Select(b => new
                {
                    b.BranchId,
                    b.BranchName
                })
                .ToListAsync();

            var suppliers = await _context.Suppliers
                .Select(s => new
                {
                    s.SupplierId,
                    s.SupplierName
                })
                .ToListAsync();

            var medicines = await _context.Medicines
               .Select(m => new
               {
                   m.MedicineId,
                   m.MedicineName
               })
               .ToListAsync();

            ViewBag.Role = role;
            ViewBag.Permissions = permissions;
            ViewBag.Username = username;

            ViewBag.Branches = branches;
            ViewBag.Suppliers = suppliers;
            ViewBag.Medicines = medicines;

            return View();
        }

        [Authorize]
        [HttpGet]
        [Route("Stocks/Requests")]

        public async Task<IActionResult> StockTransfers()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var branchId = User.FindFirst("BranchId")?.Value;

            var permissions = await _permissionService.GetAllPermissionsForRolesAsync(role);

            var medicines = await _context.Medicines
                .Select(m => new { m.MedicineId, m.MedicineName })
                .ToListAsync();

            var branches = await _context.Branches
                .Where(b => b.BranchId.ToString() != branchId)
                .Select(b => new { b.BranchId, b.BranchName })
                .ToListAsync();

            var received = await _context.MedicineBatches
                .GroupBy(b => new { b.MedicineId, b.BranchId })
                .Select(g => new
                {
                    g.Key.MedicineId,
                    g.Key.BranchId,
                    ReceivedQty = g.Sum(x => x.TotalStockReceived)
                })
                .ToListAsync();

            var issued = await _context.StockLedgers
                .Where(s => s.QuantityOut > 0)
                .GroupBy(s => new { s.MedicineId, s.BranchId })
                .Select(g => new
                {
                    g.Key.MedicineId,
                    g.Key.BranchId,
                    IssuedQty = g.Sum(x => x.QuantityOut)
                })
                .ToListAsync();

            var stock = (from r in received
                         join i in issued
                         on new { r.MedicineId, r.BranchId }
                         equals new { i.MedicineId, i.BranchId }
                         into gj
                         from i in gj.DefaultIfEmpty()
                         select new
                         {
                             r.MedicineId,
                             r.BranchId,
                             Stock = r.ReceivedQty - (i?.IssuedQty ?? 0)
                         }).ToList();

            var stockMap = stock.ToDictionary(
                x => $"{x.MedicineId}_{x.BranchId}",
                x => x.Stock
            );

            ViewBag.Role = role;
            ViewBag.Permissions = permissions;
            ViewBag.Username = username;
            ViewBag.Medicines = medicines;
            ViewBag.Branches = branches;
            ViewBag.Stock = stockMap;
            ViewBag.CurrentBranch = branchId;

            return View();
        }
    }
}
