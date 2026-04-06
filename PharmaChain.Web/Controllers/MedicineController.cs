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
        private readonly IBatchService _batchService;
        public MedicineController(IUserService userService,
            IPermissionService permissionService,
            IBranchService branchService,
            IPharmaChainDbContext context,
            ILogService logService,
            IMedicineService medicineService,
            IBatchService batchService)
        {
            _userService = userService;
            _permissionService = permissionService;
            _branchService = branchService;
            _context = context;
            _logService = logService;
            _medicineService = medicineService;
            _batchService = batchService;
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
            ViewBag.TotalPages = Convert.ToInt32(record.TotalMedicines) / 5;
            ViewBag.size = size;

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var permissions = await _permissionService.GetAllPermissionsForRolesAsync(role);

            ViewBag.Role = role;
            ViewBag.Permissions = permissions;
            ViewBag.Username = username;

            return View();
        }

        [Authorize]
        [HttpGet]
        [Route("Medicine/Batch/Create-Batch")]
        public async Task<IActionResult> CreateBatch()
        {
            await PopulateBatchViewData();

            ViewBag.FormAction = "/api/Batch/Create";
            return View("MedicineBatchForm");
        }


        [Authorize]
        [HttpGet]
        [Route("Medicine/Batch/Edit-Batch")]
        public async Task<IActionResult> EditBatch(string id)
        {
            await PopulateBatchViewData();

            var batch = await _context.MedicineBatches
                .FirstOrDefaultAsync(b => b.BatchId == id);

            if (batch == null) return NotFound();

            ViewBag.FormAction = $"/api/Batch/Update?id={id}";
            return View("MedicineBatchForm", batch);
        }
        private async Task PopulateBatchViewData()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var permissions = await _permissionService.GetAllPermissionsForRolesAsync(role);

            var today = DateOnly.FromDateTime(DateTime.Today);
            var next30Days = today.AddDays(30);

            var suppliers = await _context.Suppliers
                .Select(s => new { s.SupplierId, s.SupplierName })
                .ToListAsync();

            var medicines = await _context.Medicines
                .Select(m => new { m.MedicineId, m.MedicineName })
                .ToListAsync();

            var branches = await _context.Branches
                .Select(b => new { b.BranchId, b.BranchName })
                .ToListAsync();

            var totalActiveBatches = await _context.MedicineBatches
                .CountAsync(b => b.ExpDate >= today);

            var expiringSoonBatches = await _context.MedicineBatches
                .CountAsync(b => b.ExpDate >= today && b.ExpDate <= next30Days);

            var activeSuppliers = await _context.Suppliers
                .CountAsync(s => s.IsActive);

            ViewBag.Username = username;
            ViewBag.Role = role;
            ViewBag.Permissions = permissions;

            ViewBag.Suppliers = suppliers;
            ViewBag.Medicines = medicines;
            ViewBag.Branches = branches;

            ViewBag.TotalActiveBatches = totalActiveBatches;
            ViewBag.ExpiringSoonBatches = expiringSoonBatches;
            ViewBag.ActiveSuppliers = activeSuppliers;
        }

        [Authorize]
        [HttpGet]
        [Route("Medicine/Batch/List")]
        public async Task<IActionResult> BatchList(int page = 1, int size = 10)
        {
            var result = await _batchService.GetBatchesAsync(page, size);

            var today = DateOnly.FromDateTime(DateTime.Today);
            var next30Days = today.AddDays(30);

            ViewBag.Batches = result.Records;
            ViewBag.TotalBatches = result.TotalBatches;
            ViewBag.ActiveBatches = result.Records.Count(b => b.ExpDate >= today && b.ExpDate <= next30Days == false);
            ViewBag.ExpiringSoon = result.Records.Count(b => b.ExpDate >= today && b.ExpDate <= next30Days);
            ViewBag.ExpiredBatches = result.Records.Count(b => b.ExpDate < today);
            ViewBag.page = page;
            ViewBag.size = size;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalBatches / size);

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var permissions = await _permissionService.GetAllPermissionsForRolesAsync(role);

            ViewBag.Username = username;
            ViewBag.Role = role;
            ViewBag.Permissions = permissions;

            return View("BatchList");
        }
    }
}
