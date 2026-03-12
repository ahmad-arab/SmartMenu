using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Data.Entities;
using SmartMenu.Data.Enums;
using SmartMenu.Models.MenuCommand;
using SmartMenu.Models.MenuLable;
using SmartMenu.Models.Theme;
using SmartMenu.Models.TimeTable;
using SmartMenu.Services.Category;
using SmartMenu.Services.FileUpload;
using SmartMenu.Services.Item;
using SmartMenu.Services.Language;
using SmartMenu.Services.Menu;
using SmartMenu.Services.MenuCommand;
using SmartMenu.Services.MenuLable;
using SmartMenu.Services.MenuStaff;
using SmartMenu.Services.Qr;
using SmartMenu.Services.Tenant;
using SmartMenu.Services.Theme;
using System.Security.Claims;

namespace SmartMenu.Controllers
{
    [Authorize(Roles = "TenantAdmin")]
    public class TenantAdminController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly ICategoryService _categoryService;
        private readonly IItemService _itemService;
        private readonly IMenuLableService _menuLableService;
        private readonly IMenuCommandService _menuCommandService;
        private readonly IMenuStaffService _menuStaffService;
        private readonly ILanguageService _languageService;
        private readonly IThemeService _themeService;
        private readonly IQrCodeService _qrCodeService;
        private readonly IFileUploadService _fileUploadService;
        private readonly ITenantService _tenantService;

        public TenantAdminController(
            IMenuService menuService,
            ICategoryService categoryService,
            IItemService itemService,
            IMenuLableService menuLableService,
            IMenuCommandService menuCommandService,
            IMenuStaffService menuStaffService,
            ILanguageService languageService,
            IThemeService themeService,
            IQrCodeService qrCodeService,
            IFileUploadService fileUploadService,
            ITenantService tenantService)
        {
            _menuService = menuService;
            _categoryService = categoryService;
            _itemService = itemService;
            _menuLableService = menuLableService;
            _menuCommandService = menuCommandService;
            _menuStaffService = menuStaffService;
            _languageService = languageService;
            _themeService = themeService;
            _qrCodeService = qrCodeService;
            _fileUploadService = fileUploadService;
            _tenantService = tenantService;
        }

        #region Menus

        [HttpGet]
        public async Task<IActionResult> MenusList()
        {
            int tenantId = GetTenantId();
            var model = await _menuService.GetMenuListAsync(tenantId);

            var tenant = await _tenantService.GetTenantAsync(tenantId);
            model.TenantAllowedMenusCount = tenant?.AllowedMenusCount ?? 0;
            model.TenantActualMenusCount = model.Menus.Count;
            model.IsAllowedToAddNewMenus = model.TenantActualMenusCount < model.TenantAllowedMenusCount;

            return View("Menu/MenusList", model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMenu()
        {
            int tenantId = GetTenantId();
            var model = await _menuService.GetCreateMenuModelAsync(tenantId);
            return View("Menu/CreateMenu", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenu(Models.Menu.CreateMenuViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Menu/CreateMenu", model);

            int tenantId = GetTenantId();

            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantAllowedMenusCount = tenant?.AllowedMenusCount ?? 0;
            var tenantActualMenusCount = await _menuService.GetMenusCountAsync(tenantId);
            var isAllowedToAddNewMenus = tenantActualMenusCount < tenantAllowedMenusCount;
            if(!isAllowedToAddNewMenus)
                return Json(new { success = false, message = $"Menu limit reached. Allowed: {tenantAllowedMenusCount}, Current: {tenantActualMenusCount}." });

            if (model.Image != null && !_fileUploadService.IsAllowedImageExtension(model.Image.FileName))
                return Json(new { success = false, message = "Only image files are allowed." });

            await _menuService.CreateMenuAsync(tenantId, model);
            return Json(new { success = true, message = "Menu created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditMenu(int id)
        {
            int tenantId = GetTenantId();
            var model = await _menuService.GetEditMenuModelAsync(tenantId, id);
            if (model == null)
                return Json(new { success = false, message = "Menu not found." });
            return View("Menu/EditMenu", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenu(int id, Models.Menu.EditMenuViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Menu/EditMenu", model);

            int tenantId = GetTenantId();

            if (model.Image != null && !_fileUploadService.IsAllowedImageExtension(model.Image.FileName))
                return Json(new { success = false, message = "Only image files are allowed." });

            var success = await _menuService.EditMenuAsync(tenantId, id, model);
            if (!success)
                return Json(new { success = false, message = "Menu not found." });

            return Json(new { success = true, message = "Menu updated successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> MenuPage(int id)
        {
            int tenantId = GetTenantId();
            var model = await _menuService.GetMenuPageModelAsync(tenantId, id);
            if (model == null)
                return Json(new { success = false, message = "Menu not found." });

            var tenant = await _tenantService.GetTenantAsync(tenantId);
            model.TenantIsUsingCommands = tenant?.UseCommands ?? false;

            return View("Menu/MenuPage", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            int tenantId = GetTenantId();
            var success = await _menuService.DeleteMenuAsync(tenantId, id);
            if (!success)
                return Json(new { success = false, message = "Menu not found." });
            return Json(new { success = true, message = "Menu deleted successfully." });
        }
        #endregion

        #region Theme Designer

        [HttpGet]
        public async Task<IActionResult> ThemeDesigner(int menuId)
        {
            int tenantId = GetTenantId();
            var previewUrl = Url.Action("View", "Menu", new { menuId, previewTheme = true }) ?? string.Empty;
            var vm = await _menuService.GetThemeDesignerModelAsync(tenantId, menuId, previewUrl);
            if (vm == null) return Unauthorized();
            return View("Menu/ThemeDesigner", vm);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMenuThemeDesigner(int menuId, MenuThemeKey key, bool getDefaultValues = false)
        {
            int tenantId = GetTenantId();
            var menu = await _menuService.GetMenuForThemeEditAsync(tenantId, menuId);
            if (menu == null) return Unauthorized();
            var model = _themeService.GetMenuThemeInstance(key, menu.MenuThemeJson, getDefaultValues);
            return PartialView($"~/Views/Shared/Components/MenuTheme/{key}_Designer.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadCategoryCardThemeDesigner(int menuId, CategoryCardThemeKey key, bool getDefaultValues = false)
        {
            int tenantId = GetTenantId();
            var menu = await _menuService.GetMenuForThemeEditAsync(tenantId, menuId);
            if (menu == null) return Unauthorized();
            var model = _themeService.GetCategoryCardThemeInstance(key, menu.CategoryCardThemeJson, getDefaultValues);
            return PartialView($"~/Views/Shared/Components/CategoryCardTheme/{key}_Designer.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadItemCardThemeDesigner(int menuId, ItemCardThemeKey key, bool getDefaultValues = false)
        {
            int tenantId = GetTenantId();
            var menu = await _menuService.GetMenuForThemeEditAsync(tenantId, menuId);
            if (menu == null) return Unauthorized();
            var model = _themeService.GetItemCardThemeInstance(key, menu.ItemCardThemeJson, getDefaultValues);
            return PartialView($"~/Views/Shared/Components/ItemCardTheme/{key}_Designer.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadLableThemeDesigner(int menuId, LableThemeKey key, bool getDefaultValues = false)
        {
            int tenantId = GetTenantId();
            var menu = await _menuService.GetMenuForThemeEditAsync(tenantId, menuId);
            if (menu == null) return Unauthorized();
            var model = _themeService.GetLableThemeInstance(key, menu.LableThemeJson, getDefaultValues);
            return PartialView($"~/Views/Shared/Components/LableTheme/{key}_Designer.cshtml", model);
        }

        public class SaveThemeRequest
        {
            public int MenuId { get; set; }
            public MenuThemeKey MenuThemeKey { get; set; }
            public string? MenuThemeJson { get; set; }
            public CategoryCardThemeKey CategoryCardThemeKey { get; set; }
            public string? CategoryCardThemeJson { get; set; }
            public ItemCardThemeKey ItemCardThemeKey { get; set; }
            public string? ItemCardThemeJson { get; set; }
            public LableThemeKey LableThemeKey { get; set; }
            public string? LableThemeJson { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTheme([FromForm] SaveThemeRequest request)
        {
            int tenantId = GetTenantId();
            var dto = new SaveThemeDto
            {
                MenuId = request.MenuId,
                MenuThemeKey = request.MenuThemeKey,
                MenuThemeJson = request.MenuThemeJson,
                CategoryCardThemeKey = request.CategoryCardThemeKey,
                CategoryCardThemeJson = request.CategoryCardThemeJson,
                ItemCardThemeKey = request.ItemCardThemeKey,
                ItemCardThemeJson = request.ItemCardThemeJson,
                LableThemeKey = request.LableThemeKey,
                LableThemeJson = request.LableThemeJson
            };
            var success = await _menuService.SaveThemeAsync(tenantId, dto);
            if (!success) return Unauthorized();
            return Json(new { success = true });
        }

        #endregion

        #region Categories
        [HttpGet]
        public async Task<IActionResult> CreateCategory(int menuId)
        {
            int tenantId = GetTenantId();
            var model = await _categoryService.GetCreateCategoryModelAsync(tenantId, menuId);
            return View("Category/CreateCategory", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Models.Category.CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int tenantId = GetTenantId();
                model.AvailableLanguages = (await _categoryService.GetLanguagesForTenantAsync(tenantId)).ToList();
                ViewBag.MenuId = model.MenuId;
                return View("Category/CreateCategory", model);
            }

            if (model.Image != null && !_fileUploadService.IsAllowedImageExtension(model.Image.FileName))
                return Json(new { success = false, message = "Only image files are allowed." });

            await _categoryService.CreateCategoryAsync(model);
            return Json(new { success = true, message = "Category created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            int tenantId = GetTenantId();
            var model = await _categoryService.GetEditCategoryModelAsync(tenantId, id);
            if (model == null)
                return Json(new { success = false, message = "Category not found." });
            return View("Category/EditCategory", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Models.Category.EditCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int tenantId = GetTenantId();
                model.AvailableLanguages = (await _categoryService.GetLanguagesForTenantAsync(tenantId)).ToList();
                return View("Category/EditCategory", model);
            }

            if (model.Image != null && !_fileUploadService.IsAllowedImageExtension(model.Image.FileName))
                return Json(new { success = false, message = "Only image files are allowed." });

            var success = await _categoryService.EditCategoryAsync(model);
            if (!success)
                return Json(new { success = false, message = "Category not found." });

            return Json(new { success = true, message = "Category updated successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> CategoryPage(int id)
        {
            int tenantId = GetTenantId();
            var model = await _categoryService.GetCategoryPageModelAsync(tenantId, id);
            if (model == null)
                return Json(new { success = false, message = "Category not found." });
            return View("Category/CategoryPage", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
                return Json(new { success = false, message = "Category not found." });
            return Json(new { success = true, message = "Category deleted successfully." });
        }
        #endregion

        #region Items
        [HttpGet]
        public async Task<IActionResult> CreateItem(int categoryId)
        {
            int tenantId = GetTenantId();
            var model = await _itemService.GetCreateItemModelAsync(tenantId, categoryId);
            return View("Item/CreateItem", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(Models.Item.CreateItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int tenantId = GetTenantId();
                model.AvailableLanguages = (await _itemService.GetLanguagesForTenantAsync(tenantId)).ToList();
                return View("Item/CreateItem", model);
            }

            if (model.Image != null && !_fileUploadService.IsAllowedImageExtension(model.Image.FileName))
                return Json(new { success = false, message = "Only image files are allowed." });

            await _itemService.CreateItemAsync(model);
            return Json(new { success = true, message = "Item created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditItem(int id)
        {
            int tenantId = GetTenantId();
            var model = await _itemService.GetEditItemModelAsync(tenantId, id);
            if (model == null)
                return Json(new { success = false, message = "Item not found." });
            return View("Item/EditItem", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(Models.Item.EditItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int tenantId = GetTenantId();
                model.AvailableLanguages = (await _itemService.GetLanguagesForTenantAsync(tenantId)).ToList();
                return View("Item/EditItem", model);
            }

            if (model.Image != null && !_fileUploadService.IsAllowedImageExtension(model.Image.FileName))
                return Json(new { success = false, message = "Only image files are allowed." });

            var success = await _itemService.EditItemAsync(model);
            if (!success)
                return Json(new { success = false, message = "Item not found." });

            return Json(new { success = true, message = "Item updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var success = await _itemService.DeleteItemAsync(id);
            if (!success)
                return Json(new { success = false, message = "Item not found." });
            return Json(new { success = true, message = "Item deleted successfully." });
        }
        #endregion

        #region MenuLable
        [HttpGet]
        public async Task<IActionResult> GetMenuLables(int menuId)
        {
            int tenantId = GetTenantId();
            var result = await _menuLableService.GetMenuLablesAsync(tenantId, menuId);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMenuLable(int menuId)
        {
            int tenantId = GetTenantId();
            var model = await _menuLableService.GetCreateMenuLableModelAsync(tenantId, menuId);
            return View("MenuLable/CreateMenuLable", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuLable(CreateMenuLableViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int tenantId = GetTenantId();
                var editModel = await _menuLableService.GetCreateMenuLableModelAsync(tenantId, model.MenuId);
                model.AvailableLanguages = editModel.AvailableLanguages;
                return View("MenuLable/CreateMenuLable", model);
            }

            await _menuLableService.CreateMenuLableAsync(model);
            return Json(new { success = true, message = "Menu label created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditMenuLable(int id)
        {
            int tenantId = GetTenantId();
            var model = await _menuLableService.GetEditMenuLableModelAsync(tenantId, id);
            if (model == null)
                return Json(new { success = false, message = "Menu label not found." });
            return View("MenuLable/EditMenuLable", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuLable(int id, Models.MenuLable.EditMenuLableViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int tenantId = GetTenantId();
                var refreshed = await _menuLableService.GetEditMenuLableModelAsync(tenantId, id);
                model.AvailableLanguages = refreshed?.AvailableLanguages ?? new();
                return View("MenuLable/EditMenuLable", model);
            }

            var success = await _menuLableService.EditMenuLableAsync(id, model);
            if (!success)
                return Json(new { success = false, message = "Menu label not found." });

            return Json(new { success = true, message = "Menu label updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenuLable(int id)
        {
            var success = await _menuLableService.DeleteMenuLableAsync(id);
            if (!success)
                return Json(new { success = false, message = "Menu label not found." });
            return Json(new { success = true, message = "Menu label deleted successfully." });
        }
        #endregion

        #region MenuCommand
        [HttpGet]
        public async Task<IActionResult> GetMenuCommands(int menuId)
        {
            int tenantId = GetTenantId();

            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if(!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            var result = await _menuCommandService.GetMenuCommandsAsync(tenantId, menuId);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMenuCommand(int menuId)
        {
            int tenantId = GetTenantId();

            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            var model = await _menuCommandService.GetCreateMenuCommandModelAsync(tenantId, menuId);
            return View("MenuCommand/CreateMenuCommand", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuCommand(CreateMenuCommandViewModel model)
        {
            int tenantId = GetTenantId();
            if (!ModelState.IsValid)
            {
                var refreshed = await _menuCommandService.GetCreateMenuCommandModelAsync(tenantId, model.MenuId);
                model.AvailableLanguages = refreshed.AvailableLanguages;
                model.AvailableStaff = refreshed.AvailableStaff;
                return View("MenuCommand/CreateMenuCommand", model);
            }

            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            await _menuCommandService.CreateMenuCommandAsync(model);
            return Json(new { success = true, message = "Menu command created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditMenuCommand(int id)
        {
            int tenantId = GetTenantId();

            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            var model = await _menuCommandService.GetEditMenuCommandModelAsync(tenantId, id);
            if (model == null)
                return Json(new { success = false, message = "Menu command not found." });
            return View("MenuCommand/EditMenuCommand", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuCommand(int id, EditMenuCommandViewModel model)
        {
            int tenantId = GetTenantId();
            if (!ModelState.IsValid)
            {
                var refreshed = await _menuCommandService.GetEditMenuCommandModelAsync(tenantId, id);
                model.AvailableLanguages = refreshed?.AvailableLanguages ?? new();
                model.AvailableStaff = refreshed?.AvailableStaff ?? new();
                return View("MenuCommand/EditMenuCommand", model);
            }

            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            var success = await _menuCommandService.EditMenuCommandAsync(id, model);
            if (!success)
                return Json(new { success = false, message = "Menu command not found." });

            return Json(new { success = true, message = "Menu command updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenuCommand(int id)
        {
            int tenantId = GetTenantId();
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            var success = await _menuCommandService.DeleteMenuCommandAsync(id);
            if (!success)
                return Json(new { success = false, message = "Menu command not found." });
            return Json(new { success = true, message = "Menu command deleted successfully." });
        }
        #endregion

        #region MenuStaff
        [HttpGet]
        public async Task<IActionResult> GetMenuStaffs(int menuId)
        {
            int tenantId = GetTenantId();
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            var result = await _menuStaffService.GetMenuStaffsAsync(menuId);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMenuStaff(int menuId)
        {
            int tenantId = GetTenantId();
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            var model = await _menuStaffService.GetCreateMenuStaffModelAsync(menuId);
            return View("MenuStaff/CreateMenuStaff", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuStaff(Models.MenuStaff.CreateMenuStaffViewModel model)
        {
            int tenantId = GetTenantId();
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            if (!ModelState.IsValid)
                return View("MenuStaff/CreateMenuStaff", model);

            await _menuStaffService.CreateMenuStaffAsync(model);
            return Json(new { success = true, message = "Menu staff created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditMenuStaff(int id)
        {
            int tenantId = GetTenantId();
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            var model = await _menuStaffService.GetEditMenuStaffModelAsync(id);
            if (model == null)
                return Json(new { success = false, message = "Menu staff not found." });
            return View("MenuStaff/EditMenuStaff", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuStaff(int id, Models.MenuStaff.EditMenuStaffViewModel model)
        {
            int tenantId = GetTenantId();
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            if (!ModelState.IsValid)
                return View("MenuStaff/EditMenuStaff", model);

            var success = await _menuStaffService.EditMenuStaffAsync(id, model);
            if (!success)
                return Json(new { success = false, message = "Menu staff not found." });

            return Json(new { success = true, message = "Menu staff updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenuStaff(int id)
        {
            int tenantId = GetTenantId();
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            var success = await _menuStaffService.DeleteMenuStaffAsync(id);
            if (!success)
                return Json(new { success = false, message = "Menu staff not found." });
            return Json(new { success = true, message = "Menu staff deleted successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterMenuStaff(int id)
        {
            int tenantId = GetTenantId();
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            var (success, message) = await _menuStaffService.RegisterMenuStaffAsync(id, tenantId);
            return Json(new { success, message });
        }
        #endregion

        #region TimeTable
        [HttpGet]
        public async Task<IActionResult> EditStaffTimeTable(int id)
        {
            int tenantId = GetTenantId();
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            var model = await _menuStaffService.GetStaffTimeTableModelAsync(id);
            if (model == null)
                return Json(new { success = false, message = "Menu staff not found." });
            return View("TimeTable/EditStaffTimeTable", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaffTimeTable(StaffTimeTableViewModel model)
        {
            int tenantId = GetTenantId();
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            if (!ModelState.IsValid)
                return View("TimeTable/EditStaffTimeTable", model);

            var success = await _menuStaffService.EditStaffTimeTableAsync(model);
            if (!success)
                return Json(new { success = false, message = "Menu staff not found." });

            return Json(new { success = true, message = "Timetable updated successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> BulkEditStaffTimeTable(int menuId)
        {
            int tenantId = GetTenantId();
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            var model = await _menuStaffService.GetBulkEditModelAsync(menuId);
            return View("TimeTable/BulkEditStaffTimeTable", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkEditStaffTimeTable(BulkStaffTimeTableViewModel model)
        {
            int tenantId = GetTenantId();
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var tenantIsUsingCommands = tenant?.UseCommands ?? false;
            if (!tenantIsUsingCommands)
                return Json(new { success = false, message = "Commands feature is not enabled for this tenant." });

            if (model.StaffIds == null || !model.StaffIds.Any())
                return Json(new { success = false, message = "Please select at least one staff member." });

            var success = await _menuStaffService.BulkEditStaffTimeTableAsync(model);
            if (!success)
                return Json(new { success = false, message = "No valid staff selected." });

            return Json(new { success = true, message = "Schedule applied to selected staff." });
        }
        #endregion

        #region Languages
        [HttpGet]
        public IActionResult Languages()
        {
            return View("Language/Languages");
        }

        [HttpGet]
        public async Task<IActionResult> GetLanguages()
        {
            int tenantId = GetTenantId();
            var languages = await _languageService.GetLanguagesAsync(tenantId);
            return Json(languages);
        }

        [HttpGet]
        public IActionResult CreateLanguage()
        {
            return View("Language/CreateLanguage", new Models.Language.CreateLanguageViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLanguage(Models.Language.CreateLanguageViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Language/CreateLanguage", model);

            int tenantId = GetTenantId();
            await _languageService.CreateLanguageAsync(tenantId, model);
            return Json(new { success = true, message = "Language created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditLanguage(int id)
        {
            var model = await _languageService.GetLanguageForEditAsync(id);
            if (model == null)
                return NotFound();
            return View("Language/EditLanguage", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLanguage(int id, Models.Language.EditLanguageViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Language/EditLanguage", model);

            var success = await _languageService.EditLanguageAsync(id, model);
            if (!success)
                return NotFound();

            return Json(new { success = true, message = "Language updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            var success = await _languageService.DeleteLanguageAsync(id);
            if (!success)
                return Json(new { success = false, message = "Language not found." });
            return Json(new { success = true, message = "Language deleted successfully." });
        }
        #endregion

        #region Helpers

        private int GetTenantId()
        {
            var claim = User.FindFirst("TenantId") ?? User.FindFirst(ClaimTypes.GroupSid);
            if (claim == null || !int.TryParse(claim.Value, out int tenantId))
            {
                throw new UnauthorizedAccessException("TenantId claim is missing or invalid.");
            }
            return tenantId;
        }


        #endregion

        #region Render Icon

        [HttpGet]
        public IActionResult RenderIcon(IconIdentifier icon)
        {
            return PartialView("~/Views/Shared/Icon/IconIdentifierView.cshtml", icon);
        }

        #endregion

        #region QR Code

        [HttpGet]
        public IActionResult GenerateQrCodeModal(int menuId)
        {
            ViewBag.MenuId = menuId;
            return View("Menu/GenerateQrCode");
        }

        [HttpGet]
        public async Task<IActionResult> GenerateQrPng(int menuId, string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return BadRequest("Identifier is required");

            int tenantId = GetTenantId();
            if (!await _menuService.MenuExistsForTenantAsync(tenantId, menuId))
                return Unauthorized();

            var url = Url.Action("View", "Menu", new { menuId, identifier }, Request.Scheme) ?? string.Empty;
            var png = _qrCodeService.GeneratePng(url, pixelsPerModule: 10);
            return File(png, "image/png");
        }

        #endregion
    }
}
