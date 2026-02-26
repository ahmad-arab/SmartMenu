using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;
using SmartMenu.Data.Entities;
using SmartMenu.Models.Tenant;
using SmartMenu.Models.User;
using SmartMenu.Services.FileUpload;

namespace SmartMenu.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminController(
            ILogger<AdminController> logger,
            ApplicationDbContext context,
            IFileUploadService fileUploadService,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _fileUploadService = fileUploadService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region Tenants
        [HttpGet]
        public IActionResult Tenants()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTenants()
        {
            var tenants = await _context.Tenants
                .Select(t => new TenantListViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();

            return Json(tenants);
        }

        [HttpGet]
        public IActionResult CreateTenant()
        {
            var model = new CreateTenantViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTenant(CreateTenantViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Logo != null && !_fileUploadService.IsAllowedImageExtension(model.Logo.FileName))
            {
                ModelState.AddModelError("Logo", "Only image files are allowed.");
                return View(model);
            }

            var logoUrl = await _fileUploadService.UploadImageAsync(model.Logo, "tenant-logos");

            var tenant = new Tenant
            {
                Name = model.Name,
                LogoUrl = logoUrl
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Tenant created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditTenant(int id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
                return NotFound();

            var model = new EditTenantViewModel
            {
                Id = tenant.Id,
                Name = tenant.Name,
                LogoUrl = tenant.LogoUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTenant(int id, EditTenantViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
                return NotFound();

            tenant.Name = model.Name;

            if (model.Logo != null && model.Logo.Length > 0)
            {
                if (!_fileUploadService.IsAllowedImageExtension(model.Logo.FileName))
                {
                    ModelState.AddModelError("Logo", "Only image files are allowed.");
                    model.LogoUrl = tenant.LogoUrl; // preserve current logo in view
                    return View(model);
                }

                tenant.LogoUrl = await _fileUploadService.UploadImageAsync(model.Logo, "tenant-logos");
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Tenant updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTenant(int id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
                return NotFound();

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Tenant deleted successfully." });
        }
        #endregion

        #region Users
        [HttpGet]
        public IActionResult TenantUsers(int tenantId)
        {
            return View(tenantId);
        }

        [HttpGet]
        public async Task<IActionResult> GetTenantUsers(int tenantId)
        {
            var users = await _context.ApplicationUsers
                .Where(x => x.TenantId == tenantId)
                .Select(t => new UserListViewModel
                {
                    Id = t.Id,
                    Username = t.UserName
                })
                .ToListAsync();

            return Json(users);
        }

        [HttpGet]
        public IActionResult CreateTenantUser(int tenantId)
        {
            var model = new CreateUserViewModel
            {
                TenantId = tenantId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenantUser(CreateUserViewModel user)
        {
            if (!ModelState.IsValid)
                return View(user);

            int tenantId = user.TenantId;

            // Check if username already exists
            var existingUser = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserName == user.Username);
            if (existingUser != null)
            {
                return Json(new { success = false, message = "Username already exists." });
            }

            // Create user
            var appUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = user.Username,
                Email = user.Username,
                EmailConfirmed = true,
                TenantId = tenantId
            };

            var result = await _userManager.CreateAsync(appUser, user.Password);
            if (!result.Succeeded)
            {
                return Json(new { success = false, message = string.Join("; ", result.Errors.Select(e => e.Description)) });
            }

            var tenantAdminRole = await _roleManager.FindByNameAsync("TenantAdmin");
            if (tenantAdminRole == null)
            {
                return Json(new { success = false, message = "Role 'TenantAdmin' not found" });
            }

            var userRole = new ApplicationUserRole
            {
                UserId = appUser.Id,
                RoleId = tenantAdminRole.Id
            };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "User created and added to TenantAdmin role." });
        }

        [HttpGet]
        public IActionResult ChangeUserPassword(string userId)
        {
            var model = new ChangeUserPasswordViewModel
            {
                UserId = userId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return Json(new { success = false, message = "User not found." });

            // Reset the password via a generated token (old password is not available here)
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = errors });
            }

            return Json(new { success = true, message = "Password changed successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            await _userManager.DeleteAsync(user);
            return Json(new { success = true, message = "User deleted successfully." });
        }
        #endregion
    }
}