using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.Interfaces;
using System.Security.Claims;

namespace PharmaChain.Web.Controllers
{
    public class SupplierController : Controller
    {

        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IBranchService _branchService;
        private readonly IPharmaChainDbContext _context;
        public SupplierController(IUserService userService,
            IPermissionService permissionService,
            IBranchService branchService,
            IPharmaChainDbContext context)
        {
            _userService = userService;
            _permissionService = permissionService;
            _branchService = branchService;
            _context = context;
        }

        [HttpGet]
        [Route("Supplier/Add-Supplier")]
        [Authorize]
        public async Task<IActionResult> AddSupplier()
        {
            await PopulateSupplierViewData();

            ViewBag.FormAction = "/api/Supplier/Create";
            return View("SupplierForm");
        }

        [HttpGet]
        [Route("Supplier/Edit-Supplier/")]
        [Authorize]
        public async Task<IActionResult> EditSupplier(string id)
        {
            await PopulateSupplierViewData();

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            ViewBag.FormAction = $"/api/Supplier/Update?id={id}";
            return View("SupplierForm", supplier);
        }

        private async Task PopulateSupplierViewData()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var permissions = await _permissionService.GetAllPermissionsForRolesAsync(role);

            var supplierInfos = new
            {
                TotalSuppliers = await _context.Suppliers.CountAsync(),
                ActiveSuppliers = await _context.Suppliers.CountAsync(x => x.IsActive),
                InactiveSuppliers = await _context.Suppliers.CountAsync(x => !x.IsActive)
            };

            ViewBag.Role = role;
            ViewBag.Permissions = permissions;
            ViewBag.Username = username;
            ViewBag.supplierInfos = supplierInfos;
        }
    }
}
