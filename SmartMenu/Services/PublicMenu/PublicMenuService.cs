using SmartMenu.Data.Enums;
using SmartMenu.Models.Category;
using SmartMenu.Models.MenuCommand;
using SmartMenu.Models.MenuLable;
using SmartMenu.Models.Theme;
using SmartMenu.Models.View;
using SmartMenu.Repositories.Category;
using SmartMenu.Repositories.Item;
using SmartMenu.Repositories.Language;
using SmartMenu.Repositories.Menu;
using SmartMenu.Repositories.MenuCommand;
using SmartMenu.Repositories.MenuLable;
using SmartMenu.Repositories.MenuStaff;
using SmartMenu.Services.Theme;
using SmartMenu.Services.Whatsapp;

namespace SmartMenu.Services.PublicMenu
{
    public class PublicMenuService : IPublicMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMenuLableRepository _menuLableRepository;
        private readonly IMenuCommandRepository _menuCommandRepository;
        private readonly IMenuStaffRepository _menuStaffRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IThemeService _themeService;
        private readonly IWhatsappService _whatsappService;

        public PublicMenuService(
            IMenuRepository menuRepository,
            ICategoryRepository categoryRepository,
            IItemRepository itemRepository,
            IMenuLableRepository menuLableRepository,
            IMenuCommandRepository menuCommandRepository,
            IMenuStaffRepository menuStaffRepository,
            ILanguageRepository languageRepository,
            IThemeService themeService,
            IWhatsappService whatsappService)
        {
            _menuRepository = menuRepository;
            _categoryRepository = categoryRepository;
            _itemRepository = itemRepository;
            _menuLableRepository = menuLableRepository;
            _menuCommandRepository = menuCommandRepository;
            _menuStaffRepository = menuStaffRepository;
            _languageRepository = languageRepository;
            _themeService = themeService;
            _whatsappService = whatsappService;
        }

        public async Task<PublicMenuViewModel?> GetPublicMenuViewModelAsync(
            int menuId, string? lang, string? identifier, bool previewTheme, PreviewViewModel? previewModel)
        {
            var menu = await _menuRepository.GetByIdForPublicViewAsync(menuId);
            if (menu == null)
                return null;

            ApplyPreviewTheme(menu, previewTheme, previewModel);

            var languages = (await _languageRepository.GetByTenantIdAsync(menu.TenantId)).ToList();
            var availableLanguages = languages.Select(l => new LanguageOption
            {
                Id = l.Id,
                Code = l.Name,
                Name = l.Name,
                IsRtl = l.IsRtl,
                IsDefault = l.IsDefault
            }).ToList();
            var selectedLang = ResolveLanguage(availableLanguages, lang);

            var categoryEntities = (await _categoryRepository.GetByMenuIdWithTitlesIncludingLanguageAsync(menuId)).ToList();

            var categories = categoryEntities
                .Select(c => new CategoryListItemViewModel
                {
                    Id = c.Id,
                    ImageUrl = c.ImageUrl,
                    DefaultTitle =
                        c.CategoryTitles.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                        ?? c.CategoryTitles.FirstOrDefault()?.Text
                        ?? FallbackText.NoText,
                    TitlesByLanguage = c.CategoryTitles
                        .ToDictionary(ct => ct.Language.Name, ct => ct.Text)
                }).ToList();

            var categoryIds = categories.Select(c => c.Id).ToList();
            var menuLables = (await _menuLableRepository.GetByMenuIdWithTextsAsync(menuId)).ToList();
            var menuCommands = (await _menuCommandRepository.GetByMenuIdWithTextsAndStaffsAsync(menuId)).ToList();

            var model = new PublicMenuViewModel
            {
                MenuId = menuId,
                TenantId = menu.TenantId,
                Identifier = identifier,
                MenuDefaultTitle = menu.MenuTitles.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                    ?? menu.MenuTitles.FirstOrDefault()?.Text
                    ?? FallbackText.NoText,
                MenuLogoUrl = menu.ImageUrl,
                MenuCoverUrl = menu.ImageUrl,
                Categories = categories,
                AvailableLanguages = availableLanguages,
                SelectedLanguage = selectedLang?.Code ?? "",
                IsRtl = selectedLang?.IsRtl ?? false,
                MenuLables = BuildLableViewModels(menuLables, selectedLang?.Id, languages),
                MenuCommands = BuildCommandViewModels(menuCommands, selectedLang?.Id, languages),
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

            if (_themeService.IsOnePageMenu(model.MenuThemeKey.Value))
            {
                var items = await _itemRepository.GetAvailableItemsByCategoryIdsAsync(categoryIds);
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
                            ?? FallbackText.NoText,
                        TitlesByLanguage = i.ItemTitles.ToDictionary(it => it.Language.Name, it => it.Text),
                        Price = i.Price,
                        Description = i.ItemDescriptions.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                            ?? i.ItemDescriptions.FirstOrDefault()?.Text
                            ?? FallbackText.NoText
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

            return model;
        }

        public async Task<PublicCategoryViewModel?> GetPublicCategoryViewModelAsync(
            int categoryId, string? lang, string? identifier, bool previewTheme, PreviewViewModel? previewModel)
        {
            var category = await _categoryRepository.GetByIdWithMenuTitlesAndLanguagesAsync(categoryId);
            if (category == null)
                return null;

            var menu = category.Menu;
            ApplyPreviewTheme(menu, previewTheme, previewModel);

            var languages = (await _languageRepository.GetByTenantIdAsync(menu.TenantId)).ToList();
            var availableLanguages = languages.Select(l => new LanguageOption
            {
                Id = l.Id,
                Code = l.Name,
                Name = l.Name,
                IsRtl = l.IsRtl,
                IsDefault = l.IsDefault
            }).ToList();
            var selectedLang = ResolveLanguage(availableLanguages, lang);

            var title = category.CategoryTitles.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                        ?? category.CategoryTitles.FirstOrDefault()?.Text
                        ?? FallbackText.NoText;

            var items = await _itemRepository.GetAvailableItemsByCategoryIdAsync(categoryId);
            var itemViewModels = items.Select(i => new PublicCategoryItemViewModel
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                DefaultTitle = i.ItemTitles.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                    ?? i.ItemTitles.FirstOrDefault()?.Text
                    ?? FallbackText.NoText,
                TitlesByLanguage = i.ItemTitles.ToDictionary(it => it.Language.Name, it => it.Text),
                Price = i.Price,
                Description = i.ItemDescriptions.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                    ?? i.ItemDescriptions.FirstOrDefault()?.Text
                    ?? FallbackText.NoText
            }).ToList();

            var menuLables = (await _menuLableRepository.GetByMenuIdWithTextsAsync(menu.Id)).ToList();
            var menuCommands = (await _menuCommandRepository.GetByMenuIdWithTextsAndStaffsAsync(menu.Id)).ToList();

            return new PublicCategoryViewModel
            {
                MenuId = menu.Id,
                TenantId = menu.TenantId,
                Identifier = identifier,
                MenuDefaultTitle = menu.MenuTitles.FirstOrDefault(t => t.LanguageId == selectedLang?.Id)?.Text
                    ?? menu.MenuTitles.FirstOrDefault()?.Text
                    ?? FallbackText.NoText,
                MenuLogoUrl = menu.ImageUrl,
                MenuCoverUrl = menu.ImageUrl,
                CategoryId = category.Id,
                CategoryTitle = title,
                CategoryImageUrl = category.ImageUrl,
                AvailableLanguages = availableLanguages,
                SelectedLanguage = selectedLang?.Code ?? "",
                IsRtl = selectedLang?.IsRtl ?? false,
                MenuLables = BuildLableViewModels(menuLables, selectedLang?.Id, languages),
                Items = itemViewModels,
                MenuCommands = BuildCommandViewModels(menuCommands, selectedLang?.Id, languages),
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
        }

        public async Task<(bool Success, string Message)> SendCommandAsync(SendMenuCommandRequest request)
        {
            var menuCommand = await _menuCommandRepository.GetByIdForSendAsync(request.MenuCommandId);
            if (menuCommand == null)
                return (false, "Menu command not found.");

            if (string.IsNullOrWhiteSpace(menuCommand.SystemMessage))
                return (false, "System message is empty.");

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
                var slots = await _menuStaffRepository.GetTimeSlotsByStaffIdsAndDayAsync(staffIds, day);

                targetStaffs = targetStaffs
                    .Where(s => slots.Any(ts => ts.MenuStaffId == s.Id && ts.StartTime <= timeOfDay && timeOfDay <= ts.EndTime))
                    .ToList();
            }

            if (!targetStaffs.Any())
                return (false, "No available staff found for this time.");

            try
            {
                foreach (var staff in targetStaffs)
                    await _whatsappService.SendMessageAsync(staff.PhoneNumber, finalMessage);

                return (true, "Message sent successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to send message: {ex.Message}");
            }
        }

        private void ApplyPreviewTheme(Data.Entities.Menu menu, bool previewTheme, PreviewViewModel? previewModel)
        {
            if (!previewTheme || previewModel == null) return;

            if (!string.IsNullOrEmpty(previewModel.MenuThemeJson))
            {
                menu.MenuThemeKey = previewModel.MenuThemeKey;
                menu.MenuThemeJson = previewModel.MenuThemeJson;
            }
            if (!string.IsNullOrEmpty(previewModel.CategoryCardThemeJson))
            {
                menu.CategoryCardThemeKey = previewModel.CategoryCardThemeKey;
                menu.CategoryCardThemeJson = previewModel.CategoryCardThemeJson;
            }
            if (!string.IsNullOrEmpty(previewModel.ItemCardThemeJson))
            {
                menu.ItemCardThemeKey = previewModel.ItemCardThemeKey;
                menu.ItemCardThemeJson = previewModel.ItemCardThemeJson;
            }
            if (!string.IsNullOrEmpty(previewModel.LableThemeJson))
            {
                menu.LableThemeKey = previewModel.LableThemeKey;
                menu.LableThemeJson = previewModel.LableThemeJson;
            }
        }

        private LanguageOption? ResolveLanguage(List<LanguageOption> availableLanguages, string? lang)
        {
            return availableLanguages.FirstOrDefault(l => l.Code == lang)
                ?? availableLanguages.FirstOrDefault(l => l.IsDefault)
                ?? availableLanguages.FirstOrDefault();
        }

        private List<MenuLableListItemViewModel> BuildLableViewModels(
            List<Data.Entities.MenuLable> menuLables, int? selectedLangId,
            List<Data.Entities.Language> languages)
        {
            return menuLables.Select(l =>
            {
                var defaultText =
                    l.MenuLableTexts.FirstOrDefault(t => t.LanguageId == selectedLangId)?.Text
                    ?? l.MenuLableTexts.FirstOrDefault()?.Text
                    ?? FallbackText.NoText;

                return new MenuLableListItemViewModel
                {
                    Id = l.Id,
                    Icon = l.Icon,
                    DefaultText = defaultText,
                    SummarizedDefaultText = FallbackText.Summarize(defaultText),
                    TextsByLanguage = languages.ToDictionary(
                        lang => lang.Name,
                        lang => l.MenuLableTexts.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? FallbackText.NoText)
                };
            }).ToList();
        }

        private List<MenuCommandListItemViewModel> BuildCommandViewModels(
            List<Data.Entities.MenuCommand> menuCommands, int? selectedLangId,
            List<Data.Entities.Language> languages)
        {
            return menuCommands.Select(l =>
            {
                var defaultText =
                    l.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == selectedLangId)?.Text
                    ?? l.MenuCommandTexts.FirstOrDefault()?.Text
                    ?? FallbackText.NoText;

                return new MenuCommandListItemViewModel
                {
                    Id = l.Id,
                    Icon = l.Icon,
                    DefaultText = defaultText,
                    HasCustomerMessage = l.HasCustomerMessage,
                    SystemMessage = l.SystemMessage,
                    TextsByLanguage = languages.ToDictionary(
                        lang => lang.Name,
                        lang => l.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? FallbackText.NoText)
                };
            }).ToList();
        }
    }
}
