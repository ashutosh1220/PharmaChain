using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaChain.Application.Interfaces;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace PharmaChain.Web.Controllers
{
    public class MedicineController : Controller
    {

        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IBranchService _branchService;
        private readonly IPharmaChainDbContext _context;
        private ILogService _logService;
        private readonly IMedicineService _medicineService;
        public MedicineController(IUserService userService,
            IPermissionService permissionService,
            IBranchService branchService,
            IPharmaChainDbContext context,
            ILogService logService,
            IMedicineService medicineService)
        {
            _userService = userService;
            _permissionService = permissionService;
            _branchService = branchService;
            _context = context;
            _logService = logService;
            _medicineService = medicineService;
        }

        [HttpGet]
        [Route("Medicine/Add-Medicine")]
        [Authorize]
        public async Task<IActionResult> AddMedicine()
        {
            await PopulateCommonViewData();

            ViewBag.FormAction = "/api/Medicine/Create"; 
            return View("AddMedicine"); 
        }

        [HttpGet]
        [Route("Medicine/Edit-Medicine/")]
        [Authorize]
        public async Task<IActionResult> EditMedicine(string id)
        {
            await PopulateCommonViewData();

            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();

            ViewBag.FormAction = $"/api/Medicine/Update?id={id}"; 
            return View("AddMedicine", medicine); 
        }

        private async Task PopulateCommonViewData()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var permissions = await _permissionService.GetAllPermissionsForRolesAsync(role);
            var medicineInfos = new
            {
                TotalMedicines = await _context.Medicines.CountAsync(),
                ActiveMedicines = await _context.Medicines.CountAsync(x => x.IsActive),
                PrescriptionRequired = await _context.Medicines.CountAsync(x => x.IsPrescriptionRequired)
            };

            ViewBag.Role = role;
            ViewBag.Permissions = permissions;
            ViewBag.Username = username;
            ViewBag.medicineInfos = medicineInfos;
        }


        [Authorize]
        [HttpGet]
        [Route("Medicine/List")]
        public async Task<IActionResult> MedicinesList(int page = 1, int size = 5)
        {
            var record = await _medicineService.GetMedicinesAsync(page, size);

            ViewBag.Medicines = record.Records;
            ViewBag.TotalUsers = record.TotalMedicines;
            ViewBag.ActiveUsers = record.ActiveMedicines;
            ViewBag.InactiveUsers = record.InactiveMedicines;
            ViewBag.SuspendedUsers = record.SuspendedMedicines;
            ViewBag.page = page;
            ViewBag.TotalPages = Convert.ToInt32(record.TotalMedicines)/5;
            ViewBag.size = size;

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var permissions = await _permissionService.GetAllPermissionsForRolesAsync(role);

            ViewBag.Role = role;
            ViewBag.Permissions = permissions;
            ViewBag.Username = username;

            return View();
        }
    }
}
