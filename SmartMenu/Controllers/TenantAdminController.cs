using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;
using SmartMenu.Data.Entities;
using SmartMenu.Data.Enums;
using SmartMenu.Models.Category;
using SmartMenu.Models.Item;
using SmartMenu.Models.Language;
using SmartMenu.Models.Menu;
using SmartMenu.Models.MenuCommand;
using SmartMenu.Models.MenuLable;
using SmartMenu.Models.MenuStaff;
using SmartMenu.Models.TimeTable;
using SmartMenu.Services.Whatsapp;
using System.Security.Claims;
using SmartMenu.Services.Qr; // added
using SmartMenu.Services.Theme;
using SmartMenu.Models.Theme;
using System.Text.Json;

namespace SmartMenu.Controllers
{
    [Authorize(Roles = "TenantAdmin")]
    public class TenantAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IWhatsappService _whatsappService;
        private readonly IQrCodeService _qrCodeService; // added
        private readonly IThemeService _themeService;

        public TenantAdminController(ApplicationDbContext context, IWebHostEnvironment env, IWhatsappService whatsappService, IQrCodeService qrCodeService, IThemeService themeService) // updated
        {
            _context = context;
            _env = env;
            _whatsappService = whatsappService;
            _qrCodeService = qrCodeService; // added
            _themeService = themeService;
        }

        #region Menus

        [HttpGet]
        public async Task<IActionResult> MenusList()
        {
            int tenantId = GetTenantId();

            // Get all languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .ToListAsync();

            var menus = await _context.Menus
                .Where(m => m.TenantId == tenantId)
                .Include(m => m.MenuTitles)
                .ToListAsync();

            var menusViewModel = menus.Select(m => new MenuListItemViewModel
            {
                Id = m.Id,
                ImageUrl = m.ImageUrl,
                DefaultTitle =
                    // Try to get the default language title
                    m.MenuTitles.FirstOrDefault(t => t.LanguageId == languages.FirstOrDefault(l => l.IsDefault)?.Id)?.Text
                    // If not found, fallback to the first available title
                    ?? m.MenuTitles.FirstOrDefault()?.Text
                    // If still not found, fallback to "(No title)"
                    ?? "(No title)",
                TitlesByLanguage = languages.ToDictionary(
                    lang => lang.Name,
                    lang => m.MenuTitles.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? "(No title)"
                )
            }).ToList();

            var model = new MenuListViewModel
            {
                Menus = menusViewModel
            };

            return View("Menu/MenusList", model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMenu()
        {
            int tenantId = GetTenantId();

            // Get available languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .Select(l => new LanguageListItemViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    IsRtl = l.IsRtl
                })
                .ToListAsync();

            var model = new CreateMenuViewModel()
            {
                AvailableLanguages = languages,
                Titles = languages.Select(l => new MenuTitleViewModel
                {
                    LanguageId = l.Id
                }).ToList()
            };

            return View("Menu/CreateMenu", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenu(CreateMenuViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Menu/CreateMenu", model);

            int tenantId = GetTenantId();

            string imageUrl = null;
            if (model.Image != null && model.Image.Length > 0)
            {
                var ext = Path.GetExtension(model.Image.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                if (!allowed.Contains(ext))
                {
                    return Json(new { success = false, message = "Only image files are allowed." });
                }

                var uploads = Path.Combine(_env.WebRootPath, "uploads", "menu-images");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
                imageUrl = $"/uploads/menu-images/{fileName}";
            }

            var menu = new Menu
            {
                ImageUrl = imageUrl,
                TenantId = tenantId,
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            // Add titles and descriptions for each language
            foreach (var td in model.Titles)
            {
                if (!string.IsNullOrWhiteSpace(td.Text))
                {
                    _context.MenuTitles.Add(new MenuTitle
                    {
                        MenuId = menu.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Text
                    });
                }
            }
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Menu created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditMenu(int id)
        {
            int tenantId = GetTenantId();
            var menu = await _context.Menus
                .Include(m => m.MenuTitles)
                .Where(m => m.Id == id && m.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (menu == null)
                return Json(new { success = false, message = "Menu not found." });

            // Get all languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .Select(l => new LanguageListItemViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    IsRtl = l.IsRtl
                })
                .ToListAsync();

            // Prepare titles for all languages (including new ones)
            var titles = languages.Select(lang =>
            {
                var title = menu.MenuTitles.FirstOrDefault(t => t.LanguageId == lang.Id);
                return new MenuTitleViewModel
                {
                    LanguageId = lang.Id,
                    Text = title?.Text ?? "",
                };
            }).ToList();

            var model = new EditMenuViewModel
            {
                Id = menu.Id,
                AvailableLanguages = languages,
                Titles = titles,
                ImageUrl = menu.ImageUrl
            };

            return View("Menu/EditMenu", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenu(int id, EditMenuViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Menu/EditMenu", model);

            int tenantId = GetTenantId();
            var menu = await _context.Menus
                .Include(m => m.MenuTitles)
                .Where(m => m.Id == id && m.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (menu == null)
                return Json(new { success = false, message = "Menu not found." });

            if (model.Image != null && model.Image.Length > 0)
            {
                var ext = Path.GetExtension(model.Image.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                if (!allowed.Contains(ext))
                {
                    return Json(new { success = false, message = "Only image files are allowed." });
                }

                var uploads = Path.Combine(_env.WebRootPath, "uploads", "menu-images");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
                menu.ImageUrl = $"/uploads/menu-images/{fileName}";
            }

            // Update or add titles for each language
            foreach (var td in model.Titles)
            {
                // Title
                var title = menu.MenuTitles.FirstOrDefault(t => t.LanguageId == td.LanguageId);
                if (title != null)
                {
                    title.Text = td.Text;
                }
                else if (!string.IsNullOrWhiteSpace(td.Text))
                {
                    _context.MenuTitles.Add(new MenuTitle
                    {
                        MenuId = menu.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Text
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Menu updated successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> MenuPage(int id)
        {
            int tenantId = GetTenantId();

            var menu = await _context.Menus
                .Include(m => m.MenuTitles)
                .Where(m => m.Id == id && m.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (menu == null)
                return Json(new { success = false, message = "Menu not found." });

            // Get all languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .ToListAsync();

            // Get categories for the menu
            var categories = await _context.Categories
                .Where(c => c.MenuId == menu.Id)
                .Include(c => c.CategoryTitles)
                .ToListAsync();

            var categoryViewModels = categories.Select(cat => new CategoryListItemViewModel
            {
                Id = cat.Id,
                ImageUrl = cat.ImageUrl,
                DefaultTitle =
                    // Try to get the default language title
                    cat.CategoryTitles.FirstOrDefault(t => t.LanguageId == languages.FirstOrDefault(l => l.IsDefault)?.Id)?.Text
                    // If not found, fallback to the first available title
                    ?? cat.CategoryTitles.FirstOrDefault()?.Text
                    // If still not found, fallback to "(No title)"
                    ?? "(No title)",
                TitlesByLanguage = languages.ToDictionary(
                    lang => lang.Name,
                    lang => cat.CategoryTitles.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? "(No title)"
                )
            }).ToList();

            var model = new MenuViewModel
            {
                Id = menu.Id,
                DefaultTitle = // Try to get the default language title
                    menu.MenuTitles.FirstOrDefault(t => t.LanguageId == languages.FirstOrDefault(l => l.IsDefault)?.Id)?.Text
                    // If not found, fallback to the first available title
                    ?? menu.MenuTitles.FirstOrDefault()?.Text
                    // If still not found, fallback to "(No title)"
                    ?? "(No title)",
                ImageUrl = menu.ImageUrl,
                Categories = categoryViewModels
            };

            return View("Menu/MenuPage", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            int tenantId = GetTenantId();
            var menu = await _context.Menus
                .Include(m => m.MenuTitles)
                .Include(m => m.Categorys)
                .Include(m => m.MenuLables)
                .Include(m => m.MenuCommands)
                .Include(m => m.MenuStaffs)
                .FirstOrDefaultAsync(m => m.Id == id && m.TenantId == tenantId);

            if (menu == null)
                return Json(new { success = false, message = "Menu not found." });

            // Delete child entities
            // Categories (and their children)
            foreach (var category in await _context.Categories.Where(c => c.MenuId == menu.Id).ToListAsync())
            {
                await DeleteCategoryInternal(category.Id);
            }
            // MenuTitles
            _context.MenuTitles.RemoveRange(menu.MenuTitles);
            // MenuLables and their texts
            foreach (var lable in await _context.MenuLables.Where(l => l.MenuId == menu.Id).Include(l => l.MenuLableTexts).ToListAsync())
            {
                _context.MenuLableTexts.RemoveRange(lable.MenuLableTexts);
                _context.MenuLables.Remove(lable);
            }
            // MenuCommands and their texts
            foreach (var command in await _context.MenuCommands.Where(c => c.MenuId == menu.Id).Include(c => c.MenuCommandTexts).ToListAsync())
            {
                _context.MenuCommandTexts.RemoveRange(command.MenuCommandTexts);
                _context.MenuCommands.Remove(command);
            }
            // MenuStaffs
            foreach (var staff in await _context.MenuStaffs.Include(c => c.TimeSlots).Where(c => c.MenuId == menu.Id).ToListAsync())
            {
                _context.MenuStaffTimeSlots.RemoveRange(staff.TimeSlots);
                _context.MenuStaffs.Remove(staff);
            }

            // Delete menu image
            DeleteImageFile(menu.ImageUrl);

            // Delete menu itself
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Menu deleted successfully." });
        }
        #endregion

        #region Theme Designer

        [HttpGet]
        public async Task<IActionResult> ThemeDesigner(int menuId)
        {
            int tenantId = GetTenantId();
            var menu = await _context.Menus.FirstOrDefaultAsync(m => m.Id == menuId && m.TenantId == tenantId);
            if (menu == null) return Unauthorized();

            var vm = new ThemeDesignerViewModel
            {
                MenuId = menu.Id,
                MenuThemeKey = menu.MenuThemeKey ?? MenuThemeKey.Default,
                MenuThemeModel = _themeService.GetMenuThemeInstance(menu.MenuThemeKey ?? MenuThemeKey.Default, menu.MenuThemeJson),
                CategoryCardThemeKey = menu.CategoryCardThemeKey ?? CategoryCardThemeKey.Default,
                CategoryCardThemeModel = _themeService.GetCategoryCardThemeInstance(menu.CategoryCardThemeKey ?? CategoryCardThemeKey.Default, menu.CategoryCardThemeJson),
                ItemCardThemeKey = menu.ItemCardThemeKey ?? ItemCardThemeKey.Default,
                ItemCardThemeModel = _themeService.GetItemCardThemeInstance(menu.ItemCardThemeKey ?? ItemCardThemeKey.Default, menu.ItemCardThemeJson),
                LableThemeKey = menu.LableThemeKey ?? LableThemeKey.Default,
                LableThemeModel = _themeService.GetLableThemeInstance(menu.LableThemeKey ?? LableThemeKey.Default, menu.LableThemeJson),
                PreviewUrl = Url.Action("View", "Menu", new { menuId = menu.Id, previewTheme = true }) ?? string.Empty
            };

            return View("Menu/ThemeDesigner", vm);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMenuThemeDesigner(int menuId, MenuThemeKey key, bool getDefaultValues = false)
        {
            int tenantId = GetTenantId();
            var menu = await _context.Menus.FirstOrDefaultAsync(m => m.Id == menuId && m.TenantId == tenantId);
            if (menu == null) return Unauthorized();
            var model = _themeService.GetMenuThemeInstance(key, menu.MenuThemeJson, getDefaultValues);
            return PartialView($"~/Views/Shared/Components/MenuTheme/{key}_Designer.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadCategoryCardThemeDesigner(int menuId, CategoryCardThemeKey key, bool getDefaultValues = false)
        {
            int tenantId = GetTenantId();
            var menu = await _context.Menus.FirstOrDefaultAsync(m => m.Id == menuId && m.TenantId == tenantId);
            if (menu == null) return Unauthorized();
            var model = _themeService.GetCategoryCardThemeInstance(key, menu.CategoryCardThemeJson, getDefaultValues);
            return PartialView($"~/Views/Shared/Components/CategoryCardTheme/{key}_Designer.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadItemCardThemeDesigner(int menuId, ItemCardThemeKey key, bool getDefaultValues = false)
        {
            int tenantId = GetTenantId();
            var menu = await _context.Menus.FirstOrDefaultAsync(m => m.Id == menuId && m.TenantId == tenantId);
            if (menu == null) return Unauthorized();
            var model = _themeService.GetItemCardThemeInstance(key, menu.ItemCardThemeJson, getDefaultValues);
            return PartialView($"~/Views/Shared/Components/ItemCardTheme/{key}_Designer.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadLableThemeDesigner(int menuId, LableThemeKey key, bool getDefaultValues = false)
        {
            int tenantId = GetTenantId();
            var menu = await _context.Menus.FirstOrDefaultAsync(m => m.Id == menuId && m.TenantId == tenantId);
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
            var menu = await _context.Menus.FirstOrDefaultAsync(m => m.Id == request.MenuId && m.TenantId == tenantId);
            if (menu == null) return Unauthorized();

            menu.MenuThemeKey = request.MenuThemeKey;
            menu.MenuThemeJson = request.MenuThemeJson;
            menu.CategoryCardThemeKey = request.CategoryCardThemeKey;
            menu.CategoryCardThemeJson = request.CategoryCardThemeJson;
            menu.ItemCardThemeKey = request.ItemCardThemeKey;
            menu.ItemCardThemeJson = request.ItemCardThemeJson;
            menu.LableThemeKey = request.LableThemeKey;
            menu.LableThemeJson = request.LableThemeJson;

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        #endregion

        #region Categories
        [HttpGet]
        public async Task<IActionResult> CreateCategory(int menuId)
        {
            int tenantId = GetTenantId();

            // Get available languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .Select(l => new LanguageListItemViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    IsRtl = l.IsRtl
                })
                .ToListAsync();

            var model = new CreateCategoryViewModel
            {
                MenuId = menuId,
                AvailableLanguages = languages,
                TitlesAndDescriptions = languages.Select(l => new CategoryTitleAndDescriptionViewModel
                {
                    LanguageId = l.Id
                }).ToList()
            };

            return View("Category/CreateCategory", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate languages for the view
                int tenantId = GetTenantId();
                model.AvailableLanguages = await _context.Languages
                    .Where(l => l.TenantId == tenantId)
                    .Select(l => new LanguageListItemViewModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        IsRtl = l.IsRtl
                    })
                    .ToListAsync();
                ViewBag.MenuId = model.MenuId;
                return View("Category/CreateCategory", model);
            }

            string imageUrl = null;
            if (model.Image != null && model.Image.Length > 0)
            {
                var ext = Path.GetExtension(model.Image.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                if (!allowed.Contains(ext))
                {
                    return Json(new { success = false, message = "Only image files are allowed." });
                }

                var uploads = Path.Combine(_env.WebRootPath, "uploads", "category-images");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
                imageUrl = $"/uploads/category-images/{fileName}";
            }

            var category = new Category
            {
                ImageUrl = imageUrl,
                MenuId = model.MenuId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Add titles and descriptions for each language
            foreach (var td in model.TitlesAndDescriptions)
            {
                if (!string.IsNullOrWhiteSpace(td.Text))
                {
                    _context.CategoryTitles.Add(new CategoryTitle
                    {
                        CategoryId = category.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Text
                    });
                }
                if (!string.IsNullOrWhiteSpace(td.Description))
                {
                    _context.CategoryDescriptions.Add(new CategoryDescription
                    {
                        CategoryId = category.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Description
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Category created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            int tenantId = GetTenantId();

            var category = await _context.Categories
                .Include(c => c.CategoryTitles)
                .Include(c => c.CategoryDescriptions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return Json(new { success = false, message = "Category not found." });

            // Get all languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .Select(l => new LanguageListItemViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    IsRtl = l.IsRtl
                })
                .ToListAsync();

            // Prepare titles/descriptions for all languages (including new ones)
            var titlesAndDescriptions = languages.Select(lang =>
            {
                var title = category.CategoryTitles.FirstOrDefault(t => t.LanguageId == lang.Id);
                var desc = category.CategoryDescriptions.FirstOrDefault(d => d.LanguageId == lang.Id);
                return new CategoryTitleAndDescriptionViewModel
                {
                    LanguageId = lang.Id,
                    Text = title?.Text ?? "",
                    Description = desc?.Text ?? ""
                };
            }).ToList();

            var model = new EditCategoryViewModel
            {
                CategoryId = category.Id,
                ImageUrl = category.ImageUrl,
                AvailableLanguages = languages,
                TitlesAndDescriptions = titlesAndDescriptions
            };

            return View("Category/EditCategory", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(EditCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int tenantId = GetTenantId();
                model.AvailableLanguages = await _context.Languages
                    .Where(l => l.TenantId == tenantId)
                    .Select(l => new LanguageListItemViewModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        IsRtl = l.IsRtl
                    })
                    .ToListAsync();
                return View("Category/EditCategory", model);
            }

            var category = await _context.Categories
                .Include(c => c.CategoryTitles)
                .Include(c => c.CategoryDescriptions)
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId);

            if (category == null)
                return Json(new { success = false, message = "Category not found." });

            // Handle image update
            if (model.Image != null && model.Image.Length > 0)
            {
                var ext = Path.GetExtension(model.Image.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                if (!allowed.Contains(ext))
                {
                    return Json(new { success = false, message = "Only image files are allowed." });
                }

                var uploads = Path.Combine(_env.WebRootPath, "uploads", "category-images");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
                category.ImageUrl = $"/uploads/category-images/{fileName}";
            }

            // Update or add titles/descriptions for each language
            foreach (var td in model.TitlesAndDescriptions)
            {
                // Title
                var title = category.CategoryTitles.FirstOrDefault(t => t.LanguageId == td.LanguageId);
                if (title != null)
                {
                    title.Text = td.Text;
                }
                else if (!string.IsNullOrWhiteSpace(td.Text))
                {
                    _context.CategoryTitles.Add(new CategoryTitle
                    {
                        CategoryId = category.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Text
                    });
                }

                // Description
                var desc = category.CategoryDescriptions.FirstOrDefault(d => d.LanguageId == td.LanguageId);
                if (desc != null)
                {
                    desc.Text = td.Description;
                }
                else if (!string.IsNullOrWhiteSpace(td.Description))
                {
                    _context.CategoryDescriptions.Add(new CategoryDescription
                    {
                        CategoryId = category.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Description
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Category updated successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> CategoryPage(int id)
        {
            int tenantId = GetTenantId();

            var category = await _context.Categories
                .Include(c => c.CategoryTitles)
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();

            if (category == null)
                return Json(new { success = false, message = "Category not found." });

            // Get all languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .ToListAsync();

            // Get items for the category
            var items = await _context.Items
                .Where(c => c.CategoryId == category.Id)
                .Include(c => c.ItemTitles)
                .ToListAsync();

            var itemViewModels = items.Select(item => new ItemListItemViewModel
            {
                Id = item.Id,
                ImageUrl = item.ImageUrl,
                Price = item.Price,
                IsAvailable = item.IsAvailable,
                DefaultTitle =
                    // Try to get the default language title
                    item.ItemTitles.FirstOrDefault(t => t.LanguageId == languages.FirstOrDefault(l => l.IsDefault)?.Id)?.Text
                    // If not found, fallback to the first available title
                    ?? item.ItemTitles.FirstOrDefault()?.Text
                    // If still not found, fallback to "(No title)"
                    ?? "(No title)",
                TitlesByLanguage = languages.ToDictionary(
                    lang => lang.Name,
                    lang => item.ItemTitles.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? "(No title)"
                )
            }).ToList();

            var model = new CategoryViewModel
            {
                Id = category.Id,
                MenuId = category.MenuId,
                DefaultTitle =
                    // Try to get the default language title
                    category.CategoryTitles.FirstOrDefault(t => t.LanguageId == languages.FirstOrDefault(l => l.IsDefault)?.Id)?.Text
                    // If not found, fallback to the first available title
                    ?? category.CategoryTitles.FirstOrDefault()?.Text
                    // If still not found, fallback to "(No title)"
                    ?? "(No title)",
                Title = string.Join(" / ", languages.ToDictionary(
                    lang => lang.Name,
                    lang => category.CategoryTitles.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? "(No title)"
                ).Select(p => $"{p.Key}: {p.Value}")),
                ImageUrl = category.ImageUrl,
                Items = itemViewModels
            };

            return View("Category/CategoryPage", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await DeleteCategoryInternal(id);
            return Json(result);
        }

        // Internal helper for recursive deletion
        private async Task<object> DeleteCategoryInternal(int id)
        {
            var category = await _context.Categories
                .Include(c => c.CategoryTitles)
                .Include(c => c.CategoryDescriptions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return new { success = false, message = "Category not found." };

            // Delete items in category
            foreach (var item in await _context.Items.Where(i => i.CategoryId == category.Id).ToListAsync())
            {
                await DeleteItemInternal(item.Id);
            }
            // Delete titles/descriptions
            _context.CategoryTitles.RemoveRange(category.CategoryTitles);
            _context.CategoryDescriptions.RemoveRange(category.CategoryDescriptions);

            // Delete category image
            DeleteImageFile(category.ImageUrl);

            // Delete category itself
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return new { success = true, message = "Category deleted successfully." };
        }
        #endregion

        #region Items
        [HttpGet]
        public async Task<IActionResult> CreateItem(int categoryId)
        {
            int tenantId = GetTenantId();

            // Get available languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .Select(l => new LanguageListItemViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    IsRtl = l.IsRtl
                })
                .ToListAsync();

            var model = new CreateItemViewModel
            {
                CategoryId = categoryId,
                AvailableLanguages = languages,
                TitlesAndDescriptions = languages.Select(l => new ItemTitleAndDescriptionViewModel
                {
                    LanguageId = l.Id
                }).ToList()
            };

            return View("Item/CreateItem", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(CreateItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate languages for the view
                int tenantId = GetTenantId();
                model.AvailableLanguages = await _context.Languages
                    .Where(l => l.TenantId == tenantId)
                    .Select(l => new LanguageListItemViewModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        IsRtl = l.IsRtl
                    })
                    .ToListAsync();
                return View("Itam/CreateItem", model);
            }

            string imageUrl = null;
            if (model.Image != null && model.Image.Length > 0)
            {
                var ext = Path.GetExtension(model.Image.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                if (!allowed.Contains(ext))
                {
                    return Json(new { success = false, message = "Only image files are allowed." });
                }

                var uploads = Path.Combine(_env.WebRootPath, "uploads", "item-images");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
                imageUrl = $"/uploads/item-images/{fileName}";
            }

            var item = new Item
            {
                ImageUrl = imageUrl,
                Price = model.Price,
                IsAvailable = model.IsAvailable,
                CategoryId = model.CategoryId
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            // Add titles and descriptions for each language
            foreach (var td in model.TitlesAndDescriptions)
            {
                if (!string.IsNullOrWhiteSpace(td.Text))
                {
                    _context.ItemTitles.Add(new ItemTitle
                    {
                        ItemId = item.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Text
                    });
                }
                if (!string.IsNullOrWhiteSpace(td.Description))
                {
                    _context.ItemDescriptions.Add(new ItemDescription
                    {
                        ItemId = item.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Description
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Item created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditItem(int id)
        {
            int tenantId = GetTenantId();

            var item = await _context.Items
                .Include(c => c.ItemTitles)
                .Include(c => c.ItemDescriptions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (item == null)
                return Json(new { success = false, message = "Item not found." });

            // Get all languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .Select(l => new LanguageListItemViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    IsRtl = l.IsRtl
                })
                .ToListAsync();

            // Prepare titles/descriptions for all languages (including new ones)
            var titlesAndDescriptions = languages.Select(lang =>
            {
                var title = item.ItemTitles.FirstOrDefault(t => t.LanguageId == lang.Id);
                var desc = item.ItemDescriptions.FirstOrDefault(d => d.LanguageId == lang.Id);
                return new ItemTitleAndDescriptionViewModel
                {
                    LanguageId = lang.Id,
                    Text = title?.Text ?? "",
                    Description = desc?.Text ?? ""
                };
            }).ToList();

            var model = new EditItemViewModel
            {
                ItemId = item.Id,
                ImageUrl = item.ImageUrl,
                Price = item.Price,
                IsAvailable = item.IsAvailable,
                AvailableLanguages = languages,
                TitlesAndDescriptions = titlesAndDescriptions
            };

            return View("Item/EditItem", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(EditItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int tenantId = GetTenantId();
                model.AvailableLanguages = await _context.Languages
                    .Where(l => l.TenantId == tenantId)
                    .Select(l => new LanguageListItemViewModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        IsRtl = l.IsRtl
                    })
                    .ToListAsync();
                return View("Item/EditItem", model);
            }

            var item = await _context.Items
                .Include(c => c.ItemTitles)
                .Include(c => c.ItemDescriptions)
                .FirstOrDefaultAsync(c => c.Id == model.ItemId);

            if (item == null)
                return Json(new { success = false, message = "Item not found." });

            // Handle image update
            if (model.Image != null && model.Image.Length > 0)
            {
                var ext = Path.GetExtension(model.Image.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                if (!allowed.Contains(ext))
                {
                    return Json(new { success = false, message = "Only image files are allowed." });
                }

                var uploads = Path.Combine(_env.WebRootPath, "uploads", "item-images");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
                item.ImageUrl = $"/uploads/item-images/{fileName}";
            }

            item.Price = model.Price;
            item.IsAvailable = model.IsAvailable;

            // Update or add titles/descriptions for each language
            foreach (var td in model.TitlesAndDescriptions)
            {
                // Title
                var title = item.ItemTitles.FirstOrDefault(t => t.LanguageId == td.LanguageId);
                if (title != null)
                {
                    title.Text = td.Text;
                }
                else if (!string.IsNullOrWhiteSpace(td.Text))
                {
                    _context.ItemTitles.Add(new ItemTitle
                    {
                        ItemId = item.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Text
                    });
                }

                // Description
                var desc = item.ItemDescriptions.FirstOrDefault(d => d.LanguageId == td.LanguageId);
                if (desc != null)
                {
                    desc.Text = td.Description;
                }
                else if (!string.IsNullOrWhiteSpace(td.Description))
                {
                    _context.ItemDescriptions.Add(new ItemDescription
                    {
                        ItemId = item.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Description
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Item updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var result = await DeleteItemInternal(id);
            return Json(result);
        }

        // Internal helper for item deletion
        private async Task<object> DeleteItemInternal(int id)
        {
            var item = await _context.Items
                .Include(i => i.ItemTitles)
                .Include(i => i.ItemDescriptions)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return new { success = false, message = "Item not found." };

            _context.ItemTitles.RemoveRange(item.ItemTitles);
            _context.ItemDescriptions.RemoveRange(item.ItemDescriptions);

            // Delete item image
            DeleteImageFile(item.ImageUrl);

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return new { success = true, message = "Item deleted successfully." };
        }
        #endregion

        #region MenuLable
        [HttpGet]
        public async Task<IActionResult> GetMenuLables(int menuId)
        {
            int tenantId = GetTenantId();

            // Get all languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .ToListAsync();

            // Get menu labels for the menu
            var menuLables = await _context.MenuLables
                .Where(l => l.MenuId == menuId)
                .Include(l => l.MenuLableTexts)
                .ToListAsync();

            var result = menuLables.Select(l =>
            {
                var defaultText =
                    l.MenuLableTexts.FirstOrDefault(t => t.LanguageId == languages.FirstOrDefault(lang => lang.IsDefault)?.Id)?.Text
                    ?? l.MenuLableTexts.FirstOrDefault()?.Text
                    ?? "(No text)";

                string summarizedDefaultText = defaultText.Length > 100
                    ? defaultText.Substring(0, 100) + "..."
                    : defaultText;

                return new MenuLableListItemViewModel
                {
                    Id = l.Id,
                    Icon = l.Icon,
                    DefaultText = defaultText,
                    SummarizedDefaultText = summarizedDefaultText,
                    TextsByLanguage = languages.ToDictionary(
                        lang => lang.Name,
                        lang => l.MenuLableTexts.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? "(No text)")
                };
            }).ToList();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMenuLable(int menuId)
        {
            int tenantId = GetTenantId();

            // Get available languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .Select(l => new LanguageListItemViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    IsRtl = l.IsRtl
                })
                .ToListAsync();

            var model = new CreateMenuLableViewModel
            {
                MenuId = menuId,
                AvailableLanguages = languages,
                Texts = languages.Select(l => new MenuLableTextViewModel
                {
                    LanguageId = l.Id
                }).ToList()
            };

            return View("MenuLable/CreateMenuLable", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuLable(CreateMenuLableViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int tenantId = GetTenantId();
                model.AvailableLanguages = await _context.Languages
                    .Where(l => l.TenantId == tenantId)
                    .Select(l => new LanguageListItemViewModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        IsRtl = l.IsRtl
                    })
                    .ToListAsync();
                return View("MenuLable/CreateMenuLable", model);
            }

            var menuLable = new MenuLable
            {
                MenuId = model.MenuId,
                Icon = model.Icon
            };

            _context.MenuLables.Add(menuLable);
            await _context.SaveChangesAsync();

            foreach (var text in model.Texts)
            {
                if (!string.IsNullOrWhiteSpace(text.Text))
                {
                    _context.MenuLableTexts.Add(new MenuLableText
                    {
                        MenuLableId = menuLable.Id,
                        LanguageId = text.LanguageId,
                        Text = text.Text
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Menu label created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditMenuLable(int id)
        {
            int tenantId = GetTenantId();

            var menuLable = await _context.MenuLables
                .Include(l => l.MenuLableTexts)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (menuLable == null)
                return Json(new { success = false, message = "Menu label not found." });

            // Get all languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .Select(l => new LanguageListItemViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    IsRtl = l.IsRtl
                })
                .ToListAsync();

            var texts = languages.Select(lang =>
            {
                var text = menuLable.MenuLableTexts.FirstOrDefault(t => t.LanguageId == lang.Id);
                return new MenuLableTextViewModel
                {
                    LanguageId = lang.Id,
                    Text = text?.Text ?? ""
                };
            }).ToList();

            var model = new EditMenuLableViewModel
            {
                MenuLableId = menuLable.Id,
                Icon = menuLable.Icon,
                AvailableLanguages = languages,
                Texts = texts
            };

            return View("MenuLable/EditMenuLable", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuLable(int id, EditMenuLableViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int tenantId = GetTenantId();
                model.AvailableLanguages = await _context.Languages
                    .Where(l => l.TenantId == tenantId)
                    .Select(l => new LanguageListItemViewModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        IsRtl = l.IsRtl
                    })
                    .ToListAsync();
                return View("MenuLable/EditMenuLable", model);
            }

            var menuLable = await _context.MenuLables
                .Include(l => l.MenuLableTexts)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (menuLable == null)
                return Json(new { success = false, message = "Menu label not found." });

            menuLable.Icon = model.Icon;

            foreach (var textVm in model.Texts)
            {
                var textEntity = menuLable.MenuLableTexts.FirstOrDefault(t => t.LanguageId == textVm.LanguageId);
                if (textEntity != null)
                {
                    textEntity.Text = textVm.Text;
                }
                else if (!string.IsNullOrWhiteSpace(textVm.Text))
                {
                    _context.MenuLableTexts.Add(new MenuLableText
                    {
                        MenuLableId = menuLable.Id,
                        LanguageId = textVm.LanguageId,
                        Text = textVm.Text
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Menu label updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenuLable(int id)
        {
            var menuLable = await _context.MenuLables
                .Include(l => l.MenuLableTexts)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (menuLable == null)
                return Json(new { success = false, message = "Menu label not found." });

            _context.MenuLableTexts.RemoveRange(menuLable.MenuLableTexts);
            _context.MenuLables.Remove(menuLable);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Menu label deleted successfully." });
        }
        #endregion

        #region MenuCommand
        [HttpGet]
        public async Task<IActionResult> GetMenuCommands(int menuId)
        {
            int tenantId = GetTenantId();

            // Get all languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .ToListAsync();

            // Get menu labels for the menu
            var menuCommands = await _context.MenuCommands
                .Where(l => l.MenuId == menuId)
                .Include(l => l.MenuCommandTexts)
                .Include(l => l.MenuCommandStaffs)
                .ToListAsync();

            var result = menuCommands.Select(l =>
            {
                var defaultText =
                    l.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == languages.FirstOrDefault(lang => lang.IsDefault)?.Id)?.Text
                    ?? l.MenuCommandTexts.FirstOrDefault()?.Text
                    ?? "(No text)";

                return new MenuCommandListItemViewModel
                {
                    Id = l.Id,
                    Icon = l.Icon,
                    HasCustomerMessage = l.HasCustomerMessage,
                    DefaultText = defaultText,
                    SystemMessage = l.SystemMessage,

                    TextsByLanguage = languages.ToDictionary(
                        lang => lang.Name,
                        lang => l.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? "(No text)"),
                };
            }).ToList();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMenuCommand(int menuId)
        {
            int tenantId = GetTenantId();

            // Get available languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .Select(l => new LanguageListItemViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    IsRtl = l.IsRtl
                })
                .ToListAsync();

            // Get staff for this menu
            var staff = await _context.MenuStaffs
                .Where(s => s.MenuId == menuId)
                .OrderBy(s => s.Name)
                .Select(s => new MenuStaffOptionViewModel { Id = s.Id, Name = s.Name, PhoneNumber = s.PhoneNumber })
                .ToListAsync();

            var model = new CreateMenuCommandViewModel
            {
                MenuId = menuId,
                AvailableLanguages = languages,
                AvailableStaff = staff,
                Texts = languages.Select(l => new MenuCommandTextViewModel
                {
                    LanguageId = l.Id
                }).ToList()
            };

            return View("MenuCommand/CreateMenuCommand", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuCommand(CreateMenuCommandViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int tenantId = GetTenantId();
                model.AvailableLanguages = await _context.Languages
                    .Where(l => l.TenantId == tenantId)
                    .Select(l => new LanguageListItemViewModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        IsRtl = l.IsRtl
                    })
                    .ToListAsync();
                model.AvailableStaff = await _context.MenuStaffs
                    .Where(s => s.MenuId == model.MenuId)
                    .OrderBy(s => s.Name)
                    .Select(s => new MenuStaffOptionViewModel { Id = s.Id, Name = s.Name, PhoneNumber = s.PhoneNumber })
                    .ToListAsync();
                return View("MenuCommand/CreateMenuCommand", model);
            }

            var menuCommand = new MenuCommand
            {
                MenuId = model.MenuId,
                Icon = model.Icon,
                HasCustomerMessage = model.HasCustomerMessage,
                SystemMessage = model.SystemMessage
            };

            _context.MenuCommands.Add(menuCommand);
            await _context.SaveChangesAsync();

            foreach (var text in model.Texts)
            {
                if (!string.IsNullOrWhiteSpace(text.Text))
                {
                    _context.MenuCommandTexts.Add(new MenuCommandText
                    {
                        MenuCommandId = menuCommand.Id,
                        LanguageId = text.LanguageId,
                        Text = text.Text
                    });
                }
            }

            // Save selected staff mappings
            if (model.SelectedStaffIds != null)
            {
                foreach (var staffId in model.SelectedStaffIds.Distinct())
                {
                    _context.MenuCommandStaffs.Add(new MenuCommandStaff
                    {
                        MenuCommandId = menuCommand.Id,
                        MenuStaffId = staffId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Menu command created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditMenuCommand(int id)
        {
            int tenantId = GetTenantId();

            var menuCommand = await _context.MenuCommands
                .Include(l => l.MenuCommandTexts)
                .Include(l => l.MenuCommandStaffs)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (menuCommand == null)
                return Json(new { success = false, message = "Menu command not found." });

            // Get all languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .Select(l => new LanguageListItemViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    IsRtl = l.IsRtl
                })
                .ToListAsync();

            var texts = languages.Select(lang =>
            {
                var text = menuCommand.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == lang.Id);
                return new MenuCommandTextViewModel
                {
                    LanguageId = lang.Id,
                    Text = text?.Text ?? "",
                };
            }).ToList();

            var staff = await _context.MenuStaffs
                .Where(s => s.MenuId == menuCommand.MenuId)
                .OrderBy(s => s.Name)
                .Select(s => new MenuStaffOptionViewModel { Id = s.Id, Name = s.Name, PhoneNumber = s.PhoneNumber })
                .ToListAsync();

            var model = new EditMenuCommandViewModel
            {
                MenuCommandId = menuCommand.Id,
                Icon = menuCommand.Icon,
                HasCustomerMessage = menuCommand.HasCustomerMessage,
                SystemMessage = menuCommand.SystemMessage,
                AvailableLanguages = languages,
                AvailableStaff = staff,
                SelectedStaffIds = menuCommand.MenuCommandStaffs.Select(cs => cs.MenuStaffId).ToList(),
                Texts = texts
            };

            return View("MenuCommand/EditMenuCommand", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuCommand(int id, EditMenuCommandViewModel model)
        {
            if (!ModelState.IsValid)
            {
                int tenantId = GetTenantId();
                model.AvailableLanguages = await _context.Languages
                    .Where(l => l.TenantId == tenantId)
                    .Select(l => new LanguageListItemViewModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        IsRtl = l.IsRtl
                    })
                    .ToListAsync();
                model.AvailableStaff = await _context.MenuStaffs
                    .Where(s => s.MenuId == _context.MenuCommands.Where(mc => mc.Id == id).Select(mc => mc.MenuId).FirstOrDefault())
                    .OrderBy(s => s.Name)
                    .Select(s => new MenuStaffOptionViewModel { Id = s.Id, Name = s.Name, PhoneNumber = s.PhoneNumber })
                    .ToListAsync();
                return View("MenuCommand/EditMenuCommand", model);
            }

            var menuCommand = await _context.MenuCommands
                .Include(l => l.MenuCommandTexts)
                .Include(l => l.MenuCommandStaffs)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (menuCommand == null)
                return Json(new { success = false, message = "Menu command not found." });

            menuCommand.Icon = model.Icon;
            menuCommand.HasCustomerMessage = model.HasCustomerMessage;
            menuCommand.SystemMessage = model.SystemMessage;

            foreach (var textVm in model.Texts)
            {
                var textEntity = menuCommand.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == textVm.LanguageId);
                if (textEntity != null)
                {
                    textEntity.Text = textVm.Text;
                }
                else if (!string.IsNullOrWhiteSpace(textVm.Text))
                {
                    _context.MenuCommandTexts.Add(new MenuCommandText
                    {
                        MenuCommandId = menuCommand.Id,
                        LanguageId = textVm.LanguageId,
                        Text = textVm.Text
                    });
                }
            }

            // Update staff mappings
            var currentStaffIds = menuCommand.MenuCommandStaffs.Select(s => s.MenuStaffId).ToList();
            var newStaffIds = model.SelectedStaffIds?.Distinct().ToList() ?? new List<int>();

            // Remove unselected
            foreach (var map in menuCommand.MenuCommandStaffs.Where(m => !newStaffIds.Contains(m.MenuStaffId)).ToList())
            {
                _context.MenuCommandStaffs.Remove(map);
            }
            // Add newly selected
            foreach (var sid in newStaffIds.Where(id2 => !currentStaffIds.Contains(id2)))
            {
                _context.MenuCommandStaffs.Add(new MenuCommandStaff
                {
                    MenuCommandId = menuCommand.Id,
                    MenuStaffId = sid
                });
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Menu command updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenuCommand(int id)
        {
            var menuCommand = await _context.MenuCommands
                .Include(c => c.MenuCommandTexts)
                .Include(c => c.MenuCommandStaffs)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (menuCommand == null)
                return Json(new { success = false, message = "Menu command not found." });

            _context.MenuCommandStaffs.RemoveRange(menuCommand.MenuCommandStaffs);
            _context.MenuCommandTexts.RemoveRange(menuCommand.MenuCommandTexts);
            _context.MenuCommands.Remove(menuCommand);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Menu command deleted successfully." });
        }
        #endregion

        #region MenuStaff
        [HttpGet]
        public async Task<IActionResult> GetMenuStaffs(int menuId)
        {
            int tenantId = GetTenantId();

            var menuStaffs = await _context.MenuStaffs
                .Where(l => l.MenuId == menuId)
                .ToListAsync();

            var result = menuStaffs.Select(l =>
            {
                return new MenuStaffListItemViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    PhoneNumber = l.PhoneNumber,
                };
            }).ToList();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMenuStaff(int menuId)
        {
            int tenantId = GetTenantId();

            var model = new CreateMenuStaffViewModel
            {
                MenuId = menuId,
            };

            return View("MenuStaff/CreateMenuStaff", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuStaff(CreateMenuStaffViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("MenuStaff/CreateMenuStaff", model);
            }

            var menuStaff = new MenuStaff
            {
                MenuId = model.MenuId,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
            };

            _context.MenuStaffs.Add(menuStaff);
            await _context.SaveChangesAsync();

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Menu staff created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditMenuStaff(int id)
        {
            int tenantId = GetTenantId();

            var menuStaff = await _context.MenuStaffs
                .FirstOrDefaultAsync(l => l.Id == id);

            if (menuStaff == null)
                return Json(new { success = false, message = "Menu staff not found." });

            var model = new EditMenuStaffViewModel
            {
                MenuStaffId = menuStaff.Id,
                Name = menuStaff.Name,
                PhoneNumber = menuStaff.PhoneNumber,
                IsAvailable = menuStaff.IsAvailable
            };

            return View("MenuStaff/EditMenuStaff", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuStaff(int id, EditMenuStaffViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("MenuStaff/EditMenuStaff", model);
            }

            var menuStaff = await _context.MenuStaffs
                .FirstOrDefaultAsync(l => l.Id == id);

            if (menuStaff == null)
                return Json(new { success = false, message = "Menu staff not found." });

            menuStaff.Name = model.Name;
            menuStaff.PhoneNumber = model.PhoneNumber;
            menuStaff.IsAvailable = model.IsAvailable;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Menu staff updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenuStaff(int id)
        {
            var menuStaff = await _context.MenuStaffs
                .Include(s => s.TimeSlots)
                .Include(s => s.MenuCommandStaffs)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (menuStaff == null)
                return Json(new { success = false, message = "Menu staff not found." });

            // Remove all time slots for this staff
            if (menuStaff.TimeSlots != null && menuStaff.TimeSlots.Any())
            {
                _context.MenuStaffTimeSlots.RemoveRange(menuStaff.TimeSlots);
            }

            // Remove all command mappings for this staff
            if (menuStaff.MenuCommandStaffs != null && menuStaff.MenuCommandStaffs.Any())
            {
                _context.MenuCommandStaffs.RemoveRange(menuStaff.MenuCommandStaffs);
            }

            _context.MenuStaffs.Remove(menuStaff);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Menu staff deleted successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterMenuStaff(int id)
        {
            var menuStaff = await _context.MenuStaffs
                .Include(x => x.Menu).ThenInclude(x => x.MenuTitles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (menuStaff == null)
                return Json(new { success = false, message = "Menu staff not found." });

            if (string.IsNullOrWhiteSpace(menuStaff.PhoneNumber))
            {
                return Json(new { success = false, message = "Recipient phone number is required." });
            }

            int tenantId = GetTenantId();
            // Get all languages for the tenant
            var languages = await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .ToListAsync();

            var defaultMenuTitle =
                    // Try to get the default language title
                    menuStaff.Menu.MenuTitles.FirstOrDefault(t => t.LanguageId == languages.FirstOrDefault(l => l.IsDefault)?.Id)?.Text
                    // If not found, fallback to the first available title
                    ?? menuStaff.Menu.MenuTitles.FirstOrDefault()?.Text
                    // If still not found, fallback to "(No title)"
                    ?? "(No title)";

            try
            {
                await _whatsappService.SendTemplateMessageAsync(menuStaff.PhoneNumber, defaultMenuTitle);
                return Json(new { success = true, message = "Registration message sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to send registration message: {ex.Message}" });
            }
        }
        #endregion

        #region TimeTable
        [HttpGet]
        public async Task<IActionResult> EditStaffTimeTable(int id)
        {
            int tenantId = GetTenantId();

            var menuStaff = await _context.MenuStaffs
                .Include(s => s.TimeSlots)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (menuStaff == null)
                return Json(new { success = false, message = "Menu staff not found." });

            // Map to view model
            var model = new StaffTimeTableViewModel
            {
                MenuStaffId = menuStaff.Id,
                TimeSlots = Enum.GetValues(typeof(DayOfWeek))
                    .Cast<DayOfWeek>()
                    .OrderBy(d => (int)d)
                    .Select(day => new StaffTimeTableViewModel.DayTimeSlot
                    {
                        Day = day,
                        Ranges = menuStaff.TimeSlots
                            .Where(ts => ts.DayOfWeek == day)
                            .OrderBy(ts => ts.StartTime)
                            .Select(ts => new StaffTimeTableViewModel.TimeRange
                            {
                                Start = ts.StartTime,
                                End = ts.EndTime
                            }).ToList()
                    }).ToList()
            };

            return View("TimeTable/EditStaffTimeTable", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaffTimeTable(StaffTimeTableViewModel model)
        {
            if (!ModelState.IsValid)
                return View("TimeTable/EditStaffTimeTable", model);

            var menuStaff = await _context.MenuStaffs
                .Include(s => s.TimeSlots)
                .FirstOrDefaultAsync(s => s.Id == model.MenuStaffId);

            if (menuStaff == null)
                return Json(new { success = false, message = "Menu staff not found." });

            // Remove all existing slots
            _context.MenuStaffTimeSlots.RemoveRange(menuStaff.TimeSlots);

            // Add new slots from model
            foreach (var daySlot in model.TimeSlots)
            {
                foreach (var range in daySlot.Ranges)
                {
                    // Only add valid ranges
                    if (range.Start < range.End)
                    {
                        _context.MenuStaffTimeSlots.Add(new MenuStaffTimeSlot
                        {
                            MenuStaffId = menuStaff.Id,
                            DayOfWeek = daySlot.Day,
                            StartTime = range.Start,
                            EndTime = range.End
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Timetable updated successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> BulkEditStaffTimeTable(int menuId)
        {
            // Get all staff for the menu
            var staff = await _context.MenuStaffs
                .Where(s => s.MenuId == menuId)
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                .ToListAsync();

            var model = new BulkStaffTimeTableViewModel
            {
                MenuId = menuId,
                AvailableStaff = staff,
                TimeSlots = Enum.GetValues(typeof(DayOfWeek))
                    .Cast<DayOfWeek>()
                    .OrderBy(d => (int)d)
                    .Select(d => new StaffTimeTableViewModel.DayTimeSlot { Day = d, Ranges = new List<StaffTimeTableViewModel.TimeRange>() })
                    .ToList()
            };

            return View("TimeTable/BulkEditStaffTimeTable", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkEditStaffTimeTable(BulkStaffTimeTableViewModel model)
        {
            if (model.StaffIds == null || !model.StaffIds.Any())
            {
                return Json(new { success = false, message = "Please select at least one staff member." });
            }

            // Load all selected staff for the given menu
            var staffList = await _context.MenuStaffs
                .Include(s => s.TimeSlots)
                .Where(s => s.MenuId == model.MenuId && model.StaffIds.Contains(s.Id))
                .ToListAsync();

            if (!staffList.Any())
            {
                return Json(new { success = false, message = "No valid staff selected." });
            }

            foreach (var staff in staffList)
            {
                // remove existing slots
                _context.MenuStaffTimeSlots.RemoveRange(staff.TimeSlots);

                // add new slots from model
                foreach (var daySlot in model.TimeSlots)
                {
                    if (daySlot?.Ranges == null) continue;
                    foreach (var range in daySlot.Ranges)
                    {
                        if (range.Start < range.End)
                        {
                            _context.MenuStaffTimeSlots.Add(new MenuStaffTimeSlot
                            {
                                MenuStaffId = staff.Id,
                                DayOfWeek = daySlot.Day,
                                StartTime = range.Start,
                                EndTime = range.End
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Schedule applied to selected staff." });
        }
        #endregion

        #region Languages
        [HttpGet]
        public async Task<IActionResult> Languages()
        {
            await Task.Delay(0);
            return View("Language/Languages");
        }

        [HttpGet]
        public async Task<IActionResult> GetLanguages()
        {
            int tenantId = GetTenantId();

            var languages = await _context.Languages
                .Where(x => x.TenantId == tenantId)
                .Select(t => new LanguageListItemViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    IsDefault = t.IsDefault,
                    IsRtl = t.IsRtl
                })
                .ToListAsync();

            return Json(languages);
        }

        [HttpGet]
        public IActionResult CreateLanguage()
        {
            var model = new CreateLanguageViewModel();
            return View("Language/CreateLanguage", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLanguage(CreateLanguageViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Language/CreateLanguage", model);

            int tenantId = GetTenantId();

            var language = new Language
            {
                Name = model.Name,
                IsRtl = model.IsRtl,
                IsDefault = model.IsDefault,
                TenantId = tenantId
            };

            _context.Languages.Add(language);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Language created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> EditLanguage(int id)
        {
            var language = await _context.Languages.FindAsync(id);
            if (language == null)
                return NotFound();

            var model = new EditLanguageViewModel
            {
                Id = language.Id,
                Name = language.Name,
                IsRtl = language.IsRtl,
                IsDefault = language.IsDefault
            };
            return View("Language/EditLanguage", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLanguage(int id, EditLanguageViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Language/EditLanguage", model);

            var language = await _context.Languages.FindAsync(id);
            if (language == null)
                return NotFound();

            language.Name = model.Name;
            language.IsRtl = model.IsRtl;
            language.IsDefault = model.IsDefault;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Language updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            var language = await _context.Languages
                .Include(l => l.LanguageTexts)
                .Include(l => l.CategoryTitles)
                .Include(l => l.CategoryDescriptions)
                .Include(l => l.ItemTitles)
                .Include(l => l.ItemDescriptions)
                .Include(l => l.MenuLanguages)
                .Include(l => l.MenuLableTexts)
                .Include(l => l.MenuCommandTexts)
                .Include(l => l.MenuTitles)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (language == null)
                return Json(new { success = false, message = "Language not found." });

            // Remove all child entities first
            _context.LanguageTexts.RemoveRange(language.LanguageTexts);
            _context.CategoryTitles.RemoveRange(language.CategoryTitles);
            _context.CategoryDescriptions.RemoveRange(language.CategoryDescriptions);
            _context.ItemTitles.RemoveRange(language.ItemTitles);
            _context.ItemDescriptions.RemoveRange(language.ItemDescriptions);
            _context.MenuLanguages.RemoveRange(language.MenuLanguages);
            _context.MenuLableTexts.RemoveRange(language.MenuLableTexts);
            _context.MenuCommandTexts.RemoveRange(language.MenuCommandTexts);
            _context.MenuTitles.RemoveRange(language.MenuTitles);

            // Remove the language itself
            _context.Languages.Remove(language);
            await _context.SaveChangesAsync();

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

        private void DeleteImageFile(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;
            // Remove leading slash if present
            var relativePath = imageUrl.StartsWith("/") ? imageUrl.Substring(1) : imageUrl;
            var fullPath = Path.Combine(_env.WebRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        #endregion

        #region Render Icon

        [HttpGet]
        public IActionResult RenderIcon(IconIdentifier icon)
        {
            // Renders the partial view with the icon as the model
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
            var menu = await _context.Menus.FirstOrDefaultAsync(m => m.Id == menuId && m.TenantId == tenantId);
            if (menu == null)
                return Unauthorized();

            // Build absolute URL to public menu with identifier param
            var url = Url.Action("View", "Menu", new { menuId = menuId, identifier = identifier }, Request.Scheme) ?? string.Empty;

            var png = _qrCodeService.GeneratePng(url, pixelsPerModule: 10);
            return File(png, "image/png");
        }

        #endregion
    }
}