using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaChain.Application.Interfaces;
using System.Security.Claims;

namespace PharmaChain.Web.Controllers
{
    public class LogsController : Controller
    {

        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IBranchService _branchService;
        private readonly IPharmaChainDbContext _context;
        private ILogService _logService;
        public LogsController(IUserService userService,
            IPermissionService permissionService,
            IBranchService branchService,
            IPharmaChainDbContext context,
            ILogService logService)
        {
            _userService = userService;
            _permissionService = permissionService;
            _branchService = branchService;
            _context = context;
            _logService = logService;
        }

        [HttpGet]
        [Route("Logs")]
        [Authorize]
        public async Task<IActionResult> ViewLogs(int page = 1, int size = 10)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var permissions = await _permissionService.GetAllPermissionsForRolesAsync(role);
            var logs = await _logService.GetLogsAsync(page, size);

            ViewBag.Logs = logs.Logs;
            ViewBag.TotalPages = logs.TotalPages;
            ViewBag.TotalLogs = logs.TotalLogs;
            ViewBag.CurrentPage = logs.CurrentPage;

            ViewBag.Role = role;
            ViewBag.Permissions = permissions;
            ViewBag.Username = username;

            return View();
        }
    }
}
