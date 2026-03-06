using SmartMenu.Data.Entities;
using SmartMenu.Models.Language;
using SmartMenu.Models.MenuLable;
using SmartMenu.Repositories.Language;
using SmartMenu.Repositories.MenuLable;

namespace SmartMenu.Services.MenuLable
{
    public class MenuLableService : IMenuLableService
    {
        private readonly IMenuLableRepository _menuLableRepository;
        private readonly ILanguageRepository _languageRepository;

        public MenuLableService(IMenuLableRepository menuLableRepository, ILanguageRepository languageRepository)
        {
            _menuLableRepository = menuLableRepository;
            _languageRepository = languageRepository;
        }

        public async Task<IEnumerable<MenuLableListItemViewModel>> GetMenuLablesAsync(int tenantId, int menuId)
        {
            var languages = (await _languageRepository.GetByTenantIdAsync(tenantId)).ToList();
            var menuLables = await _menuLableRepository.GetByMenuIdWithTextsAsync(menuId);
            var defaultLangId = languages.FirstOrDefault(l => l.IsDefault)?.Id;

            return menuLables.Select(l =>
            {
                var defaultText =
                    l.MenuLableTexts.FirstOrDefault(t => t.LanguageId == defaultLangId)?.Text
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
            });
        }

        public async Task<CreateMenuLableViewModel> GetCreateMenuLableModelAsync(int tenantId, int menuId)
        {
            var languages = await GetLanguageViewModelsAsync(tenantId);
            return new CreateMenuLableViewModel
            {
                MenuId = menuId,
                AvailableLanguages = languages.ToList(),
                Texts = languages.Select(l => new MenuLableTextViewModel { LanguageId = l.Id }).ToList()
            };
        }

        public async Task CreateMenuLableAsync(CreateMenuLableViewModel model)
        {
            var menuLable = new Data.Entities.MenuLable
            {
                MenuId = model.MenuId,
                Icon = model.Icon
            };
            await _menuLableRepository.AddAsync(menuLable);

            foreach (var text in model.Texts.Where(t => !string.IsNullOrWhiteSpace(t.Text)))
            {
                await _menuLableRepository.AddTextAsync(new MenuLableText
                {
                    MenuLableId = menuLable.Id,
                    LanguageId = text.LanguageId,
                    Text = text.Text
                });
            }
        }

        public async Task<EditMenuLableViewModel?> GetEditMenuLableModelAsync(int tenantId, int id)
        {
            var menuLable = await _menuLableRepository.GetByIdWithTextsAsync(id);
            if (menuLable == null)
                return null;

            var languages = await GetLanguageViewModelsAsync(tenantId);

            var texts = languages.Select(lang => new MenuLableTextViewModel
            {
                LanguageId = lang.Id,
                Text = menuLable.MenuLableTexts.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? ""
            }).ToList();

            return new EditMenuLableViewModel
            {
                MenuLableId = menuLable.Id,
                Icon = menuLable.Icon,
                AvailableLanguages = languages.ToList(),
                Texts = texts
            };
        }

        public async Task<bool> EditMenuLableAsync(int id, EditMenuLableViewModel model)
        {
            var menuLable = await _menuLableRepository.GetByIdWithTextsAsync(id);
            if (menuLable == null)
                return false;

            menuLable.Icon = model.Icon;

            foreach (var textVm in model.Texts)
            {
                var textEntity = menuLable.MenuLableTexts.FirstOrDefault(t => t.LanguageId == textVm.LanguageId);
                if (textEntity != null)
                    textEntity.Text = textVm.Text;
                else if (!string.IsNullOrWhiteSpace(textVm.Text))
                    await _menuLableRepository.AddTextAsync(new MenuLableText
                    {
                        MenuLableId = menuLable.Id,
                        LanguageId = textVm.LanguageId,
                        Text = textVm.Text
                    });
            }

            await _menuLableRepository.UpdateAsync(menuLable);
            return true;
        }

        public async Task<bool> DeleteMenuLableAsync(int id)
        {
            var menuLable = await _menuLableRepository.GetByIdWithTextsAsync(id);
            if (menuLable == null)
                return false;

            await _menuLableRepository.RemoveTextsRangeAsync(menuLable.MenuLableTexts);
            await _menuLableRepository.DeleteAsync(menuLable);
            return true;
        }

        private async Task<IEnumerable<LanguageListItemViewModel>> GetLanguageViewModelsAsync(int tenantId)
        {
            var languages = await _languageRepository.GetByTenantIdAsync(tenantId);
            return languages.Select(l => new LanguageListItemViewModel
            {
                Id = l.Id,
                Name = l.Name,
                IsRtl = l.IsRtl
            });
        }
    }
}
