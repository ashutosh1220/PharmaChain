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
        [Route("StockTransaction/PurchaseInvoice")]
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
    }
}
