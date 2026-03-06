using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Models.Tenant;
using SmartMenu.Models.User;
using SmartMenu.Services.FileUpload;
using SmartMenu.Services.Tenant;
using SmartMenu.Services.User;

namespace SmartMenu.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ITenantService _tenantService;
        private readonly IUserService _userService;
        private readonly IFileUploadService _fileUploadService;

        public AdminController(
            ILogger<AdminController> logger,
            ITenantService tenantService,
            IUserService userService,
            IFileUploadService fileUploadService)
        {
            _logger = logger;
            _tenantService = tenantService;
            _userService = userService;
            _fileUploadService = fileUploadService;
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
            var tenants = await _tenantService.GetAllTenantsAsync();
            return Json(tenants);
        }

        [HttpGet]
        public IActionResult CreateTenant()
        {
            return View(new CreateTenantViewModel());
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

            await _tenantService.CreateTenantAsync(model);
            return Json(new { success = true, message = "Tenant created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditTenant(int id)
        {
            var model = await _tenantService.GetTenantForEditAsync(id);
            if (model == null)
                return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTenant(int id, EditTenantViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Logo != null && model.Logo.Length > 0 && !_fileUploadService.IsAllowedImageExtension(model.Logo.FileName))
            {
                ModelState.AddModelError("Logo", "Only image files are allowed.");
                var existing = await _tenantService.GetTenantForEditAsync(id);
                model.LogoUrl = existing?.LogoUrl;
                return View(model);
            }

            var success = await _tenantService.UpdateTenantAsync(id, model);
            if (!success)
                return NotFound();

            return Json(new { success = true, message = "Tenant updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTenant(int id)
        {
            var success = await _tenantService.DeleteTenantAsync(id);
            if (!success)
                return NotFound();
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
            var users = await _userService.GetUsersByTenantAsync(tenantId);
            return Json(users);
        }

        [HttpGet]
        public IActionResult CreateTenantUser(int tenantId)
        {
            return View(new CreateUserViewModel { TenantId = tenantId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenantUser(CreateUserViewModel user)
        {
            if (!ModelState.IsValid)
                return View(user);

            var (success, message) = await _userService.CreateTenantUserAsync(user);
            return Json(new { success, message });
        }

        [HttpGet]
        public IActionResult ChangeUserPassword(string userId)
        {
            return View(new ChangeUserPasswordViewModel { UserId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, message) = await _userService.ChangePasswordAsync(model);
            return Json(new { success, message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var (success, message) = await _userService.DeleteUserAsync(id);
            return Json(new { success, message });
        }
        #endregion
    }
}