using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;
using SmartMenu.Data.Entities;
using SmartMenu.Data.Enums;
using SmartMenu.Models.Category;
using SmartMenu.Models.Menu;
using SmartMenu.Models.MenuCommand;
using SmartMenu.Models.MenuLable;
using SmartMenu.Models.Theme;
using SmartMenu.Models.View;
using SmartMenu.Services.Theme;
using SmartMenu.Services.Whatsapp;

namespace SmartMenu.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IWhatsappService _whatsappService;
        private readonly IThemeService _themeService;

        public MenuController(ApplicationDbContext context, IWebHostEnvironment env, IWhatsappService whatsappService, IThemeService themeService)
        {
            _context = context;
            _env = env;
            _whatsappService = whatsappService;
            _themeService = themeService;
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> View(int menuId, string? lang = null, string? identifier = null, bool previewTheme = false, [FromForm] PreviewViewModel? previewModel = null)
        {
            var menu = await _context.Menus
                .Include(m => m.MenuTitles)
                .ThenInclude(mt => mt.Language)
                .FirstOrDefaultAsync(x => x.Id == menuId);

            if(previewTheme && previewModel != null && !string.IsNullOrEmpty(previewModel.MenuThemeJson))
            {
                menu.MenuThemeKey = previewModel.MenuThemeKey;
                menu.MenuThemeJson = previewModel.MenuThemeJson;
            }
            if (previewTheme && previewModel != null && !string.IsNullOrEmpty(previewModel.CategoryCardThemeJson))
            {
                menu.CategoryCardThemeKey = previewModel.CategoryCardThemeKey;
                menu.CategoryCardThemeJson = previewModel.CategoryCardThemeJson;
            }
            if (previewTheme && previewModel != null && !string.IsNullOrEmpty(previewModel.ItemCardThemeJson))
            {
                menu.ItemCardThemeKey = previewModel.ItemCardThemeKey;
                menu.ItemCardThemeJson = previewModel.ItemCardThemeJson;
            }
            if (previewTheme && previewModel != null && !string.IsNullOrEmpty(previewModel.LableThemeJson))
            {
                menu.LableThemeKey = previewModel.LableThemeKey;
                menu.LableThemeJson = previewModel.LableThemeJson;
            }

            var availableLanguages = _context.Languages
                .Where(l => l.TenantId == menu.TenantId)
                .Select(l => new LanguageOption
                {
                    Id = l.Id,
                    Code = l.Name,
                    Name = l.Name,
                    IsRtl = l.IsRtl,
                    IsDefault = l.IsDefault
                }).ToList();

            var selectedLang =
                   availableLanguages.FirstOrDefault(l => l.Code == lang)
                ?? availableLanguages.FirstOrDefault(l => l.IsDefault)
                ?? availableLanguages.FirstOrDefault();

            var categoryEntities = await _context.Categories
                .Include(c => c.CategoryTitles).ThenInclude(ct => ct.Language)
                .Where(c => c.MenuId == menuId)
                .ToListAsync();

            var categories = categoryEntities
                .Select(c => {
                    var defaultText =
                    c.CategoryTitles.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                    ?? c.CategoryTitles.FirstOrDefault()?.Text
                    ?? "(No text)";

                    return new CategoryListItemViewModel
                    {
                        Id = c.Id,
                        ImageUrl = c.ImageUrl,
                        DefaultTitle = defaultText
                    };
                }).ToList();

            var categoryIds = categories.Select(c => c.Id);
            var categoryTitles = _context.CategoryTitles
                .Include(ct => ct.Language)
                .Where(ct => categoryIds.Contains(ct.CategoryId))
                .ToList();
            foreach (var category in categories)
            {
                var titles = categoryTitles.Where(ct => ct.CategoryId == category.Id);
                category.TitlesByLanguage = titles
                        .Select(ct => new KeyValuePair<string, string>(ct.Language.Name, ct.Text))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            // Fetch MenuLables for this menu
            var menuLables = await _context.MenuLables
                .Where(l => l.MenuId == menuId)
                .Include(l => l.MenuLableTexts)
                .ToListAsync();

            // Fetch MenuCommands for this menu
            var menuCommands = await _context.MenuCommands
                .Where(l => l.MenuId == menuId)
                .Include(l => l.MenuCommandTexts)
                .ToListAsync();

            var languages = _context.Languages
                .Where(l => l.TenantId == menu.TenantId)
                .ToList();

            var menuLableViewModels = menuLables.Select(l =>
            {
                var defaultText =
                    l.MenuLableTexts.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
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

            var menuCommandViewModels = menuCommands.Select(l =>
            {
                var defaultText =
                    l.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                    ?? l.MenuCommandTexts.FirstOrDefault()?.Text
                    ?? "(No text)";

                return new MenuCommandListItemViewModel
                {
                    Id = l.Id,
                    Icon = l.Icon,
                    DefaultText = defaultText,
                    HasCustomerMessage = l.HasCustomerMessage,
                    SystemMessage = l.SystemMessage,
                    TextsByLanguage = languages.ToDictionary(
                        lang => lang.Name,
                        lang => l.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? "(No text)")
                };
            }).ToList();

            var model = new PublicMenuViewModel
            {
                MenuId = menuId,
                Identifier = identifier,
                MenuDefaultTitle = menu.MenuTitles.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                    ?? menu.MenuTitles.FirstOrDefault()?.Text
                    ?? "(No text)",
                MenuLogoUrl = menu.ImageUrl,
                MenuCoverUrl = menu.ImageUrl,
                Categories = categories,
                AvailableLanguages = availableLanguages,
                SelectedLanguage = selectedLang?.Code ?? "",
                IsRtl = selectedLang?.IsRtl ?? false,
                MenuLables = menuLableViewModels,
                MenuCommands = menuCommandViewModels,
                MenuThemeKey = menu.MenuThemeKey ?? MenuThemeKey.Default,
                MenuThemeJson = menu.MenuThemeJson,
                CategoryCardThemeKey = menu.CategoryCardThemeKey ?? CategoryCardThemeKey.Default,
                CategoryCardThemeJson = menu.CategoryCardThemeJson,
                ItemCardThemeKey = menu.ItemCardThemeKey ?? ItemCardThemeKey.Default,
                ItemCardThemeJson = menu.ItemCardThemeJson,
                LableThemeKey = menu.LableThemeKey ?? LableThemeKey.Default,
                LableThemeJson = menu.LableThemeJson,
                IsThemePreview = previewTheme
            };

            // If theme is one-page, fetch items grouped by category
            var themeKey = model.MenuThemeKey.Value;
            if (_themeService.IsOnePageMenu(themeKey))
            {
                var items = await _context.Items
                    .Include(i => i.ItemTitles).ThenInclude(it => it.Language)
                    .Include(i => i.ItemDescriptions).ThenInclude(id => id.Language)
                    .Where(i => categoryIds.Contains(i.CategoryId) && i.IsAvailable)
                    .ToListAsync();

                var grouped = items.GroupBy(i => i.CategoryId).ToList();

                foreach (var grp in grouped)
                {
                    var catInfo = categories.FirstOrDefault(c => c.Id == grp.Key);
                    var catEntity = categoryEntities.FirstOrDefault(c => c.Id == grp.Key);
                    if (catInfo == null || catEntity == null) continue;

                    var itemVMs = grp.Select(i => new PublicCategoryItemViewModel
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        DefaultTitle = i.ItemTitles.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                            ?? i.ItemTitles.FirstOrDefault()?.Text
                            ?? "(No text)",
                        TitlesByLanguage = i.ItemTitles.ToDictionary(it => it.Language.Name, it => it.Text),
                        Price = i.Price,
                        Description = i.ItemDescriptions.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                            ?? i.ItemDescriptions.FirstOrDefault()?.Text
                            ?? "(No text)"
                    }).ToList();

                    model.CategoriesWithItems.Add(new MenuCategoryWithItemsViewModel
                    {
                        CategoryId = grp.Key,
                        CategoryTitle = catInfo.DefaultTitle,
                        CategoryImageUrl = catEntity.ImageUrl,
                        Items = itemVMs
                    });
                }
            }

            // If a fully custom theme view exists, render it, else fallback to default
            var explicitViewName = ResolveThemeViewName("View", model.MenuThemeKey.Value);
            return View(explicitViewName, model);
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> Category(int categoryId, string? lang = null, string? identifier = null, bool previewTheme = false, [FromForm] PreviewViewModel? previewModel = null)
        {
            var category = await _context.Categories
                .Include(c => c.CategoryTitles).ThenInclude(ct => ct.Language)
                .Include(c => c.Menu).ThenInclude(m => m.MenuTitles).ThenInclude(mt => mt.Language)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
                return NotFound();

            var menu = category.Menu;

            if (previewTheme && previewModel != null && !string.IsNullOrEmpty(previewModel.MenuThemeJson))
            {
                menu.MenuThemeKey = previewModel.MenuThemeKey;
                menu.MenuThemeJson = previewModel.MenuThemeJson;
            }
            if (previewTheme && previewModel != null && !string.IsNullOrEmpty(previewModel.CategoryCardThemeJson))
            {
                menu.CategoryCardThemeKey = previewModel.CategoryCardThemeKey;
                menu.CategoryCardThemeJson = previewModel.CategoryCardThemeJson;
            }
            if (previewTheme && previewModel != null && !string.IsNullOrEmpty(previewModel.ItemCardThemeJson))
            {
                menu.ItemCardThemeKey = previewModel.ItemCardThemeKey;
                menu.ItemCardThemeJson = previewModel.ItemCardThemeJson;
            }
            if (previewTheme && previewModel != null && !string.IsNullOrEmpty(previewModel.LableThemeJson))
            {
                menu.LableThemeKey = previewModel.LableThemeKey;
                menu.LableThemeJson = previewModel.LableThemeJson;
            }

            var availableLanguages = _context.Languages
                .Where(l => l.TenantId == menu.TenantId)
                .Select(l => new LanguageOption
                {
                    Id = l.Id,
                    Code = l.Name,
                    Name = l.Name,
                    IsRtl = l.IsRtl,
                    IsDefault = l.IsDefault
                }).ToList();

            var selectedLang =
                   availableLanguages.FirstOrDefault(l => l.Code == lang)
                ?? availableLanguages.FirstOrDefault(l => l.IsDefault)
                ?? availableLanguages.FirstOrDefault();

            var titlesByLanguage = category.CategoryTitles
                .ToDictionary(ct => ct.Language.Name, ct => ct.Text);

            var title = category.CategoryTitles.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                        ?? category.CategoryTitles.FirstOrDefault()?.Text
                        ?? "(No text)";

            // Only available items, include titles and descriptions
            var items = await _context.Items
                .Include(i => i.ItemTitles).ThenInclude(it => it.Language)
                .Include(i => i.ItemDescriptions).ThenInclude(id => id.Language)
                .Where(i => i.CategoryId == categoryId && i.IsAvailable)
                .ToListAsync();

            var itemViewModels = items.Select(i =>
            {
                var itemTitles = i.ItemTitles.ToDictionary(it => it.Language.Name, it => it.Text);
                var itemDescriptions = i.ItemDescriptions.ToDictionary(it => it.Language.Name, it => it.Text ?? "");

                return new PublicCategoryItemViewModel
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    DefaultTitle = i.ItemTitles.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                        ?? i.ItemTitles.FirstOrDefault()?.Text
                        ?? "(No text)",
                    TitlesByLanguage = itemTitles,
                    Price = i.Price,
                    Description = i.ItemDescriptions.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                        ?? i.ItemDescriptions.FirstOrDefault()?.Text
                        ?? "(No text)"
                };
            }).ToList();

            // Fetch MenuLables for this menu
            var menuLables = await _context.MenuLables
                .Where(l => l.MenuId == menu.Id)
                .Include(l => l.MenuLableTexts)
                .ToListAsync();

            // Fetch MenuCommands for this menu
            var menuCommands = await _context.MenuCommands
                .Where(l => l.MenuId == menu.Id)
                .Include(l => l.MenuCommandTexts)
                .ToListAsync();

            var languages = _context.Languages
                .Where(l => l.TenantId == menu.TenantId)
                .ToList();

            var menuLableViewModels = menuLables.Select(l =>
            {
                var defaultText =
                    l.MenuLableTexts.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
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

            var menuCommandViewModels = menuCommands.Select(l =>
            {
                var defaultText =
                    l.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                    ?? l.MenuCommandTexts.FirstOrDefault()?.Text
                    ?? "(No text)";

                return new MenuCommandListItemViewModel
                {
                    Id = l.Id,
                    Icon = l.Icon,
                    DefaultText = defaultText,
                    HasCustomerMessage = l.HasCustomerMessage,
                    SystemMessage = l.SystemMessage,
                    TextsByLanguage = languages.ToDictionary(
                        lang => lang.Name,
                        lang => l.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? "(No text)")
                };
            }).ToList();

            var model = new PublicCategoryViewModel
            {
                MenuId = menu.Id,
                Identifier = identifier,
                MenuDefaultTitle = menu.MenuTitles.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                    ?? menu.MenuTitles.FirstOrDefault()?.Text
                    ?? "(No text)",
                MenuLogoUrl = menu.ImageUrl,
                MenuCoverUrl = menu.ImageUrl,
                CategoryId = category.Id,
                CategoryTitle = title,
                CategoryImageUrl = category.ImageUrl,
                AvailableLanguages = availableLanguages,
                SelectedLanguage = selectedLang.Code,
                IsRtl = selectedLang.IsRtl,
                MenuLables = menuLableViewModels,
                Items = itemViewModels,
                MenuCommands = menuCommandViewModels,
                MenuThemeKey = menu.MenuThemeKey ?? MenuThemeKey.Default,
                MenuThemeJson = menu.MenuThemeJson,
                CategoryCardThemeKey = menu.CategoryCardThemeKey ?? CategoryCardThemeKey.Default,
                CategoryCardThemeJson = menu.CategoryCardThemeJson,
                ItemCardThemeKey = menu.ItemCardThemeKey ?? ItemCardThemeKey.Default,
                ItemCardThemeJson = menu.ItemCardThemeJson,
                LableThemeKey = menu.LableThemeKey ?? LableThemeKey.Default,
                LableThemeJson = menu.LableThemeJson,
                IsThemePreview = previewTheme
            };

            var explicitViewName = ResolveThemeViewName("Category", model.MenuThemeKey.Value);
            return View(explicitViewName, model);
        }

        private string ResolveThemeViewName(string baseName, MenuThemeKey themeKey)
        {
            var key = themeKey.ToString();
            var candidate = $"~/Views/Menu/{key}/{baseName}.cshtml";
            return candidate;
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

            var menuCommand = await _context.MenuCommands
                .Include(mc => mc.Menu)
                .Include(mc => mc.MenuCommandStaffs).ThenInclude(mcs => mcs.MenuStaff)
                .FirstOrDefaultAsync(mc => mc.Id == request.MenuCommandId);

            if (menuCommand == null)
            {
                return Json(new { success = false, message = "Menu command not found." });
            }

            if (string.IsNullOrWhiteSpace(menuCommand.SystemMessage))
            {
                return Json(new { success = false, message = "System message is empty." });
            }

            var identifierText = string.IsNullOrWhiteSpace(request.Identifier) ? "" : $"Table: {request.Identifier}\n";
            var commandText = (menuCommand.MenuCommandTexts.FirstOrDefault()?.Text ?? menuCommand.SystemMessage) + "\n";
            var customerText = string.IsNullOrWhiteSpace(request.CustomerMessage) ? "" : $"Customer: {request.CustomerMessage}\n";
            var finalMessage = $"{identifierText}{commandText}{customerText}".Trim();

            var whenLocal = request.FiredAt.AddMinutes((request.TimezoneOffsetMinutes ?? 0) * -1);
            var day = whenLocal.DayOfWeek;
            var timeOfDay = whenLocal.TimeOfDay;

            var targetStaffs = menuCommand.MenuCommandStaffs
                .Select(m => m.MenuStaff)
                .Where(s => s != null && s.IsAvailable && !string.IsNullOrWhiteSpace(s.PhoneNumber))
                .ToList();

            if (targetStaffs.Any())
            {
                var staffIds = targetStaffs.Select(s => s.Id).ToList();
                var slots = await _context.MenuStaffTimeSlots
                    .Where(ts => staffIds.Contains(ts.MenuStaffId) && ts.DayOfWeek == day)
                    .ToListAsync();

                targetStaffs = targetStaffs
                    .Where(s => slots.Any(ts => ts.MenuStaffId == s.Id && ts.StartTime <= timeOfDay && timeOfDay <= ts.EndTime))
                    .ToList();
            }

            if (!targetStaffs.Any())
            {
                return Json(new { success = false, message = "No available staff found for this time." });
            }

            try
            {
                foreach (var staff in targetStaffs)
                {
                    await _whatsappService.SendMessageAsync(staff.PhoneNumber, finalMessage);
                }
                return Json(new { success = true, message = "Message sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to send message: {ex.Message}" });
            }
        }

        #region Render Icon

        [HttpGet]
        public IActionResult RenderIcon(IconIdentifier icon)
        {
            // Renders the partial view with the icon as the model
            return PartialView("~/Views/Shared/Icon/IconIdentifierView.cshtml", icon);
        }

        #endregion
    }
}