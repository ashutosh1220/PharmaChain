using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaChain.Application.Interfaces;
using System.Security.Claims;

namespace PharmaChain.Web.Controllers
{

    public class RolesController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IBranchService _branchService;
        private readonly IPharmaChainDbContext _context;
        private IRoleService _roleService;
        public RolesController(IUserService userService,
            IPermissionService permissionService,
            IBranchService branchService,
            IPharmaChainDbContext context,
            IRoleService roleService)
        {
            _userService = userService;
            _permissionService = permissionService;
            _branchService = branchService;
            _context = context;
            _roleService = roleService;
        }

        [Authorize]
        [HttpGet]
        [Route("Role-List")]
        public async Task<IActionResult> ViewRoles(int page = 1, int size = 5)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var permissions = await _permissionService.GetAllPermissionsForRolesAsync(role);
            var AllRoles = await _roleService.GetRolesAsync(page, size);

            ViewBag.Role = role;
            ViewBag.Permissions = permissions;
            ViewBag.Username = username;
            ViewBag.AllRoles = AllRoles;
            return View();
        }
    }
}
