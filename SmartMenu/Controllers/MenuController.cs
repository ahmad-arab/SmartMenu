using Microsoft.AspNetCore.Mvc;
using SmartMenu.Data.Enums;
using SmartMenu.Models.MenuCommand;
using SmartMenu.Models.Theme;
using SmartMenu.Services.PublicMenu;
using SmartMenu.Services.Tenant;

namespace SmartMenu.Controllers
{
    public class MenuController : Controller
    {
        private readonly IPublicMenuService _publicMenuService;
        private readonly ITenantService _tenantService;

        public MenuController(IPublicMenuService publicMenuService, ITenantService tenantService)
        {
            _publicMenuService = publicMenuService;
            _tenantService = tenantService;
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> View(int menuId, string? lang = null, string? identifier = null, bool previewTheme = false, [FromForm] PreviewViewModel? previewModel = null)
        {
            var model = await _publicMenuService.GetPublicMenuViewModelAsync(menuId, lang, identifier, previewTheme, previewModel);
            if (model == null)
                return NotFound();

            int tenantId = model.TenantId;
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            model.TenantIsUsingCommands = tenant?.UseCommands ?? false;

            var explicitViewName = ResolveThemeViewName("View", model.MenuThemeKey.Value);
            return View(explicitViewName, model);
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> Category(int categoryId, string? lang = null, string? identifier = null, bool previewTheme = false, [FromForm] PreviewViewModel? previewModel = null)
        {
            var model = await _publicMenuService.GetPublicCategoryViewModelAsync(categoryId, lang, identifier, previewTheme, previewModel);
            if (model == null)
                return NotFound();

            int tenantId = model.TenantId;
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            model.TenantIsUsingCommands = tenant?.UseCommands ?? false;

            var explicitViewName = ResolveThemeViewName("Category", model.MenuThemeKey.Value);
            return View(explicitViewName, model);
        }

        private string ResolveThemeViewName(string baseName, MenuThemeKey themeKey)
        {
            var key = themeKey.ToString();
            return $"~/Views/Menu/{key}/{baseName}.cshtml";
        }

        [HttpGet]
        public IActionResult ChangeLanguage(int menuId, string lang, string? identifier = null)
        {
            return RedirectToAction("View", new { menuId, lang, identifier });
        }

        [HttpPost]
        public async Task<IActionResult> SendCommand([FromBody] SendMenuCommandRequest request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request." });

            var (success, message) = await _publicMenuService.SendCommandAsync(request);
            return Json(new { success, message });
        }

        #region Render Icon

        [HttpGet]
        public IActionResult RenderIcon(IconIdentifier icon)
        {
            return PartialView("~/Views/Shared/Icon/IconIdentifierView.cshtml", icon);
        }

        #endregion
    }
}
