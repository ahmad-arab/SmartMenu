using SmartMenu.Data.Entities;
using SmartMenu.Data.Enums;
using SmartMenu.Models.Category;
using SmartMenu.Models.Language;
using SmartMenu.Models.Menu;
using SmartMenu.Models.Theme;
using SmartMenu.Repositories.Category;
using SmartMenu.Repositories.Item;
using SmartMenu.Repositories.Language;
using SmartMenu.Repositories.Menu;
using SmartMenu.Services.FileUpload;
using SmartMenu.Services.Theme;

namespace SmartMenu.Services.Menu
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IThemeService _themeService;

        public MenuService(
            IMenuRepository menuRepository,
            ICategoryRepository categoryRepository,
            IItemRepository itemRepository,
            ILanguageRepository languageRepository,
            IFileUploadService fileUploadService,
            IThemeService themeService)
        {
            _menuRepository = menuRepository;
            _categoryRepository = categoryRepository;
            _itemRepository = itemRepository;
            _languageRepository = languageRepository;
            _fileUploadService = fileUploadService;
            _themeService = themeService;
        }

        public async Task<MenuListViewModel> GetMenuListAsync(int tenantId)
        {
            var languages = await _languageRepository.GetByTenantIdAsync(tenantId);
            var menus = await _menuRepository.GetByTenantIdWithTitlesAsync(tenantId);

            var defaultLangId = languages.FirstOrDefault(l => l.IsDefault)?.Id;

            var menusViewModel = menus.Select(m => new MenuListItemViewModel
            {
                Id = m.Id,
                ImageUrl = m.ImageUrl,
                DefaultTitle =
                    m.MenuTitles.FirstOrDefault(t => t.LanguageId == defaultLangId)?.Text
                    ?? m.MenuTitles.FirstOrDefault()?.Text
                    ?? FallbackText.NoTitle,
                TitlesByLanguage = languages.ToDictionary(
                    lang => lang.Name,
                    lang => m.MenuTitles.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? FallbackText.NoTitle)
            }).ToList();

            return new MenuListViewModel { Menus = menusViewModel };
        }

        public async Task<int> GetMenusCountAsync(int tenantId)
        {
            return await _menuRepository.GetCountByTenantIdAsync(tenantId);
        }

        public async Task<CreateMenuViewModel> GetCreateMenuModelAsync(int tenantId)
        {
            var languages = await _languageRepository.GetByTenantIdAsync(tenantId);
            var languageVMs = languages.Select(l => new LanguageListItemViewModel
            {
                Id = l.Id,
                Name = l.Name,
                IsRtl = l.IsRtl
            }).ToList();

            return new CreateMenuViewModel
            {
                AvailableLanguages = languageVMs,
                Titles = languageVMs.Select(l => new MenuTitleViewModel { LanguageId = l.Id }).ToList()
            };
        }

        public async Task CreateMenuAsync(int tenantId, CreateMenuViewModel model)
        {
            var imageUrl = await _fileUploadService.UploadImageAsync(model.Image, "menu-images");

            var menu = new Data.Entities.Menu
            {
                ImageUrl = imageUrl,
                TenantId = tenantId,
                CreateDate = DateTime.Now,
            };
            await _menuRepository.AddAsync(menu);

            foreach (var td in model.Titles.Where(t => !string.IsNullOrWhiteSpace(t.Text)))
            {
                await _menuRepository.AddTitleAsync(new MenuTitle
                {
                    MenuId = menu.Id,
                    LanguageId = td.LanguageId,
                    Text = td.Text
                });
            }
        }

        public async Task<EditMenuViewModel?> GetEditMenuModelAsync(int tenantId, int id)
        {
            var menu = await _menuRepository.GetByIdWithTitlesAsync(id, tenantId);
            if (menu == null)
                return null;

            var languages = await _languageRepository.GetByTenantIdAsync(tenantId);
            var languageVMs = languages.Select(l => new LanguageListItemViewModel
            {
                Id = l.Id,
                Name = l.Name,
                IsRtl = l.IsRtl
            }).ToList();

            var titles = languageVMs.Select(lang => new MenuTitleViewModel
            {
                LanguageId = lang.Id,
                Text = menu.MenuTitles.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? ""
            }).ToList();

            return new EditMenuViewModel
            {
                Id = menu.Id,
                AvailableLanguages = languageVMs,
                Titles = titles,
                ImageUrl = menu.ImageUrl
            };
        }

        public async Task<bool> EditMenuAsync(int tenantId, int id, EditMenuViewModel model)
        {
            var menu = await _menuRepository.GetByIdWithTitlesAsync(id, tenantId);
            if (menu == null)
                return false;

            if (model.Image?.Length > 0)
                menu.ImageUrl = await _fileUploadService.UploadImageAsync(model.Image, "menu-images");

            foreach (var td in model.Titles)
            {
                var title = menu.MenuTitles.FirstOrDefault(t => t.LanguageId == td.LanguageId);
                if (title != null)
                {
                    title.Text = td.Text;
                }
                else if (!string.IsNullOrWhiteSpace(td.Text))
                {
                    await _menuRepository.AddTitleAsync(new MenuTitle
                    {
                        MenuId = menu.Id,
                        LanguageId = td.LanguageId,
                        Text = td.Text
                    });
                }
            }

            await _menuRepository.UpdateAsync(menu);
            return true;
        }

        public async Task<Models.Menu.MenuViewModel?> GetMenuPageModelAsync(int tenantId, int id)
        {
            var menu = await _menuRepository.GetByIdWithTitlesAsync(id, tenantId);
            if (menu == null)
                return null;

            var languages = await _languageRepository.GetByTenantIdAsync(tenantId);
            var categories = await _categoryRepository.GetByMenuIdWithTitlesAsync(id);

            var defaultLangId = languages.FirstOrDefault(l => l.IsDefault)?.Id;

            var categoryViewModels = categories.Select(cat => new CategoryListItemViewModel
            {
                Id = cat.Id,
                ImageUrl = cat.ImageUrl,
                DefaultTitle =
                    cat.CategoryTitles.FirstOrDefault(t => t.LanguageId == defaultLangId)?.Text
                    ?? cat.CategoryTitles.FirstOrDefault()?.Text
                    ?? FallbackText.NoTitle,
                TitlesByLanguage = languages.ToDictionary(
                    lang => lang.Name,
                    lang => cat.CategoryTitles.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? FallbackText.NoTitle)
            }).ToList();

            return new Models.Menu.MenuViewModel
            {
                Id = menu.Id,
                DefaultTitle =
                    menu.MenuTitles.FirstOrDefault(t => t.LanguageId == defaultLangId)?.Text
                    ?? menu.MenuTitles.FirstOrDefault()?.Text
                    ?? FallbackText.NoTitle,
                ImageUrl = menu.ImageUrl,
                Categories = categoryViewModels
            };
        }

        public async Task<bool> DeleteMenuAsync(int tenantId, int id)
        {
            var menu = await _menuRepository.GetByIdForDeleteAsync(id, tenantId);
            if (menu == null)
                return false;

            // Delete all categories and their children
            var categories = await _categoryRepository.GetByMenuIdForDeleteAsync(id);
            foreach (var category in categories)
            {
                var fullCategory = await _categoryRepository.GetByIdWithTitlesAndDescriptionsAsync(category.Id);
                if (fullCategory != null)
                {
                    // Delete items
                    var items = (await _itemRepository.GetByCategoryIdWithTitlesAsync(category.Id)).ToList();
                    foreach (var item in items)
                    {
                        var fullItem = await _itemRepository.GetByIdWithTitlesAndDescriptionsAsync(item.Id);
                        if (fullItem != null)
                        {
                            await _itemRepository.RemoveTitlesRangeAsync(fullItem.ItemTitles);
                            await _itemRepository.RemoveDescriptionsRangeAsync(fullItem.ItemDescriptions);
                            _fileUploadService.DeleteImage(fullItem.ImageUrl);
                            await _itemRepository.DeleteAsync(fullItem);
                        }
                    }
                    await _categoryRepository.RemoveTitlesRangeAsync(fullCategory.CategoryTitles);
                    await _categoryRepository.RemoveDescriptionsRangeAsync(fullCategory.CategoryDescriptions);
                    _fileUploadService.DeleteImage(fullCategory.ImageUrl);
                    await _categoryRepository.DeleteAsync(fullCategory);
                }
            }

            // Delete menu titles, labels, commands, staff
            await _menuRepository.RemoveTitlesRangeAsync(menu.MenuTitles);

            _fileUploadService.DeleteImage(menu.ImageUrl);
            await _menuRepository.DeleteAsync(menu);
            return true;
        }

        public async Task<ThemeDesignerViewModel?> GetThemeDesignerModelAsync(int tenantId, int menuId, string previewUrl)
        {
            var menu = await _menuRepository.GetByIdAsync(menuId);
            if (menu == null || menu.TenantId != tenantId)
                return null;

            return new ThemeDesignerViewModel
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
                PreviewUrl = previewUrl
            };
        }

        public async Task<Data.Entities.Menu?> GetMenuForThemeEditAsync(int tenantId, int menuId)
        {
            var menu = await _menuRepository.GetByIdAsync(menuId);
            if (menu == null || menu.TenantId != tenantId)
                return null;
            return menu;
        }

        public async Task<bool> SaveThemeAsync(int tenantId, SaveThemeDto dto)
        {
            var menu = await _menuRepository.GetByIdAsync(dto.MenuId);
            if (menu == null || menu.TenantId != tenantId)
                return false;

            menu.MenuThemeKey = dto.MenuThemeKey;
            menu.MenuThemeJson = dto.MenuThemeJson;
            menu.CategoryCardThemeKey = dto.CategoryCardThemeKey;
            menu.CategoryCardThemeJson = dto.CategoryCardThemeJson;
            menu.ItemCardThemeKey = dto.ItemCardThemeKey;
            menu.ItemCardThemeJson = dto.ItemCardThemeJson;
            menu.LableThemeKey = dto.LableThemeKey;
            menu.LableThemeJson = dto.LableThemeJson;

            await _menuRepository.UpdateAsync(menu);
            return true;
        }

        public async Task<bool> MenuExistsForTenantAsync(int tenantId, int menuId)
        {
            var menu = await _menuRepository.GetByIdAsync(menuId);
            return menu != null && menu.TenantId == tenantId;
        }
    }
}
