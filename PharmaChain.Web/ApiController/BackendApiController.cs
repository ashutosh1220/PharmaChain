using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using PharmaChain.Application.DTOs;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.BackendServices;
using PharmaChain.Infrastructure.Models;
using static PharmaChain.Application.DTOs.SupplierRequest;

namespace PharmaChain.Web.ApiController
{
    [Route("api")]
    [ApiController]
    public class BackendApiController : ControllerBase
    {
        private readonly IPharmaChainDbContext _context;
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly IJwtService _jwtService;
        private readonly ILogService _logService;
        private readonly IMedicineService _medicineService;
        private readonly ISupplierService _supplierService;

        public BackendApiController(IPharmaChainDbContext context,
            IAuthService authService, IOtpService otpService,
            IUserService userService, IRoleService roleService,
            IPermissionService permissionService,
            IJwtService jwtService,
            ILogService logService,
            IMedicineService medicineService,
            ISupplierService supplierService
            )
        {
            _context = context;
            _authService = authService;
            _otpService = otpService;
            _userService = userService;
            _roleService = roleService;
            _permissionService = permissionService;
            _jwtService = jwtService;
            _logService = logService;
            _medicineService = medicineService;
            _supplierService = supplierService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var records = _context.Users.ToList();
            return Ok(records);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginsRequest request)
        {
            var user = await _authService.ValidateUserAsync(request);
            if (user == null)
                return Unauthorized("Invalid username or password");

            var token = _jwtService.GenerateToken(user);

            Response.Cookies.Append(
                "jwtToken",
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.Now.AddDays(1)
                }
            );

            return RedirectToAction("AdminDashboard", "Home");
        }

        // --- OTP helpers ---
        private string GenerateOtp(int length = 6)
        {
            var random = new Random();
            string otp = "";
            for (int i = 0; i < length; i++)
                otp += random.Next(0, 10);
            return otp;
        }

        private void SendOtpEmail(string toEmail, string otp)
        {
            var fromEmail = "hideashutosh@gmail.com";
            var appPassword = "huuffcwnnobufrjb";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("PharmaChain", fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Your OTP Code";
            message.Body = new TextPart("html")
            {
                Text = $@"
                <html>
                <body style='font-family: Arial;'>
                    <h3>Your OTP is <span style='color: #1a73e8;'>{otp}</span></h3>
                    <p>This OTP is valid for 5 minutes.</p>
                </body>
                </html>"
            };

            using var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            client.Authenticate(fromEmail, appPassword);
            client.Send(message);
            client.Disconnect(true);
        }

        [HttpPost("send-otp")]
        public IActionResult SendOtp([FromForm] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required.");

            string otp = GenerateOtp();
            _otpService.StoreOtp(email, otp, DateTime.UtcNow.AddMinutes(5)); // store in singleton service

            try
            {
                SendOtpEmail(email, otp);
                return Ok(new { Message = "OTP sent to email.", Email = email, OTP = otp });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to send OTP.", Error = ex.Message });
            }
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromForm] VerifyOtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
                return BadRequest("Email and OTP are required.");

            if (_otpService.TryValidateOtp(request.Email, request.Otp))
                return Ok(new { Message = "OTP verified successfully." });

            return Unauthorized(new { Message = "Invalid or expired OTP." });
        }


        [Authorize]
        [HttpPost("Create-User")]
        public async Task<IActionResult> CreateUserAsync([FromForm] CreateUserRequest dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();
                return new JsonResult(new { success = false, message = string.Join(", ", errors) });
            }

            try
            {
                // Separate folders
                var profileFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profileimg");
                var idProofFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "idproofimg");

                if (!Directory.Exists(profileFolder))
                    Directory.CreateDirectory(profileFolder);

                if (!Directory.Exists(idProofFolder))
                    Directory.CreateDirectory(idProofFolder);

                string? profileImageFileName = null;
                string? idProofFileName = null;

                if (dto.ProfileImage != null)
                    profileImageFileName = Guid.NewGuid() + Path.GetExtension(dto.ProfileImage.FileName);

                if (dto.IdProofDoc != null)
                    idProofFileName = Guid.NewGuid() + Path.GetExtension(dto.IdProofDoc.FileName);

                var user = new User
                {
                    UserId = GenerateUsername(),
                    FullName = dto.FullName,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    Gender = dto.Gender,
                    AddressLine1 = dto.AddressLine1,
                    AddressLine2 = dto.AddressLine2,
                    Country = dto.Country,
                    State = dto.State,
                    City = dto.City,
                    Pincode = dto.Pincode,
                    BranchId = dto.BranchId,
                    RoleId = dto.RoleId,
                    DateOfBirth = dto.DateOfBirth,
                    IdProofType = dto.IdProofType,
                    IdProofNumber = dto.IdProofNumber,
                    IdProofDocumentPath = idProofFileName != null ? "/uploads/idproofimg/" + idProofFileName : null,
                    ProfilePhotoPath = profileImageFileName != null ? "/uploads/profileimg/" + profileImageFileName : null,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                };

                var login = new Login
                {
                    UserId = user.UserId,
                    Username = user.Email,
                    PasswordHash = dto.Password,
                    IsLocked = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                _context.Logins.Add(login);

                await _context.SaveChangesAsync(HttpContext.RequestAborted);

                // Save files after DB success
                if (dto.ProfileImage != null && profileImageFileName != null)
                {
                    var filePath = Path.Combine(profileFolder, profileImageFileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await dto.ProfileImage.CopyToAsync(stream);
                }

                if (dto.IdProofDoc != null && idProofFileName != null)
                {
                    var filePath = Path.Combine(idProofFolder, idProofFileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await dto.IdProofDoc.CopyToAsync(stream);
                }

                return new JsonResult(new { success = true, message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Error creating user: " + (ex.InnerException?.Message ?? ex.Message)
                });
            }
        }

        public static string GenerateUsername()
        {
            const string prefix = "USR";
            Random random = new Random();

            int length = random.Next(6, 13);
            char[] digits = new char[length];

            for (int i = 0; i < length; i++)
            {
                digits[i] = (char)('0' + random.Next(0, 10));
            }

            return prefix + new string(digits);
        }

        [HttpPost]
        [Route("Users-List")]
        public async Task<IActionResult> UsersList(int page, int size)
        {
            var record = await _userService.GetUsersAsync(page, size);
            return Ok(record);
        }

        [HttpPost]
        [Route("AddRole")]
        public async Task<IActionResult> AddRoleAsync(string RollName)
        {
            await _roleService.AddRoleAsync(RollName);
            return Ok(RollName);
        }

        [HttpGet]
        [Route("Roles")]
        public async Task<IActionResult> Roles()
        {
            var result = await _roleService.GetRolesWithPermissionsAsync();
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateRoleState")]
        public async Task<IActionResult> UpdateRoleState(int roleid, string roleName)
        {
            var result = await _roleService.UpdateRoleStateAsync(roleid, roleName);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateActivateRoleState")]
        public async Task<IActionResult> UpdateRoleActiveState(int roleid, string roleName)
        {
            var result = await _roleService.ToggleRoleActiveStateAsync(roleid, roleName);
            return Ok(result);
        }

        [HttpGet]
        [Route("Get-Role-Permissions")]
        public async Task<IActionResult> GetRolePermissions(string roleName)
        {
            try
            {
                if (string.IsNullOrEmpty(roleName))
                {
                    return BadRequest("Role name is required.");
                }

                var rolePermissions = _permissionService.GetAllPermissionsForRolesAsync(roleName).Result;

                if (rolePermissions.Count < 1)
                {
                    return NotFound("No record found!");
                }

                return Ok(rolePermissions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("Permissions/save")]
        public async Task<IActionResult> UpdateRolePermissions([FromBody] UpdateRolePermissionRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.RoleName))
                    return BadRequest("Role name is required.");

                if (request.PermissionIds == null || !request.PermissionIds.Any())
                    return BadRequest("At least one permission is required.");

                var result = await _permissionService.UpdateRolePermissionsAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch("toggle-user-status")]
        public async Task<IActionResult> ToggleActive(string id)
        {
            try
            {
                var result = await _userService.ToggleUserActiveAsync(id);
                var status = await _context.Users
                                           .Where(x => x.UserId == id)
                                           .Select(x => x.IsActive)
                                           .FirstOrDefaultAsync();

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "User status toggled successfully",
                    Data = status
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error"
                });
            }
        }

        [HttpPatch("update-user-info")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserRequest req)
        {
            var res = await _userService.UpdateUserInfoAsync(
                req.UserId,
                req.ColumnName,
                req.UpdatedValue
                );

            return Ok(res);
        }

        [HttpGet("get-all-roles")]
        public async Task<IActionResult> GetRoles(int? page, int? size)
        {
            page ??= 1;
            size ??= 5;
            if (size > 50)
            {
                size = 5;
            }
            var roles = await _roleService.GetRolesAsync(page ??= 1, size ??= 5);
            return Ok(roles);
        }

        [HttpGet("get-logs")]
        public async Task<IActionResult> GetLogs(int? page, int? size)
        {
            try
            {
                int currentPage = page ?? 1;
                int pageSize = size ?? 10;

                if (pageSize > 50)
                    pageSize = 10;

                var logs = await _logService.GetLogsAsync(currentPage, pageSize);

                return Ok(logs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error. Please try again."
                });
            }
        }


        [HttpGet("get-log-by-id")]
        public async Task<IActionResult> GetLogByLogIdAsync(long LogId)
        {
            try
            {
                var log = await _logService.GetLogByLogIdAsync(LogId);

                return Ok(log);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error. Please try again."
                });
            }
        }



        /*************************************** Medicine ***********************************************/

        [Authorize]
        [HttpPost("Medicine/Create")]
        public async Task<IActionResult> CreateMedicine([FromForm] MedicineRequest request)
        {

            if (!string.IsNullOrEmpty(request.MedicineId))
                return BadRequest(new { success = false, message = "MedicineId should not be provided for new medicines" });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();
                return new JsonResult(new { success = false, message = string.Join(", ", errors) });
            }

            var result = await _medicineService.CreateMedicine(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Medicine/Update")]
        public async Task<IActionResult> UpdateMedicine([FromForm] MedicineRequest request)
        {
            var result = await _medicineService.UpdateMedicine(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("Medicine/Delete")]
        public async Task<IActionResult> DeleteMedicine(string id)
        {
            var result = await _medicineService.DeleteMedicine(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPatch("Medicine/Toggle/Status")]
        public async Task<IActionResult> ToggleActiveMedicine(string id)
        {
            var result = await _medicineService.ToggleActiveMedicine(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("Medicine/Get/List")]
        public async Task<IActionResult> GetMedicines(int page = 1, int size = 10)
        {
            var result = await _medicineService.GetMedicinesAsync(page, size);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("Medicine/Get")]
        public async Task<IActionResult> GetMedicineById(string id)
        {
            var result = await _medicineService.GetMedicineByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }


        /*************************************** Supplier ***********************************************/

        [Authorize]
        [HttpPost("Supplier/Create")]
        public async Task<IActionResult> CreateSupplier([FromForm] CreateSupplierRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                return new JsonResult(new
                {
                    success = false,
                    message = string.Join(", ", errors)
                });
            }

            var result = await _supplierService.CreateSupplierAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}