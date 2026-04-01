using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.Models;
using PharmaChain.Infrastructure.Services;
using System.Security.Claims;

namespace PharmaChain.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IBranchService _branchService;
        private readonly IPharmaChainDbContext _context;
        public HomeController(IUserService userService, 
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
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        [Route("Dashboard")]
        public async Task<IActionResult> AdminDashboard()
        {
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
        [Route("Registration")]
        public async Task<IActionResult> Registration()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var permissions = await _permissionService.GetAllPermissionsForRolesAsync(role);
            var branches = await _branchService.GetAllBranchesAsync();
            ViewBag.Role = role;
            ViewBag.Permissions = permissions;
            ViewBag.Username = username;
            ViewBag.Branches = branches;
            return View();
        }

        [Authorize]
        [HttpGet]
        [Route("Users-List")]
        public async Task<IActionResult> UsersList(int page = 1, int size = 5)
        {
            var record = await _userService.GetUsersAsync(page, size);

            ViewBag.Users = record.Users;
            ViewBag.TotalUsers = record.TotalUsers;
            ViewBag.ActiveUsers = record.ActiveUsers;
            ViewBag.InactiveUsers = record.InactiveUsers;
            ViewBag.SuspendedUsers = record.SuspendedUsers;
            ViewBag.page = record.CurrentPage;
            ViewBag.TotalPages = record.TotalPages;
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
        [Route("Users/Profile")]
        public async Task<IActionResult> UserProfile(string user)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var permissions = await _permissionService.GetAllPermissionsForRolesAsync(role);
            var branches = await _branchService.GetAllBranchesAsync();

            var userinfo = await _context.Users
                .Include(u => u.Role)   
                .Include(u => u.Branch)  
                .FirstOrDefaultAsync(x => x.UserId == user);

            if (userinfo == null)
            {
                return NotFound();
            }

            ViewBag.Username = username;
            ViewBag.Role = role;
            ViewBag.Branches = branches;
            ViewBag.Permissions = permissions;
            ViewBag.User = userinfo;
            ViewBag.UserRole = userinfo.Role;
            ViewBag.UserBranch = userinfo.Branch;

            return View();
        }
    }
}