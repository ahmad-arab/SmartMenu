using SmartMenu.Data.Entities;
using SmartMenu.Models.Language;
using SmartMenu.Models.MenuCommand;
using SmartMenu.Repositories.Language;
using SmartMenu.Repositories.MenuCommand;
using SmartMenu.Repositories.MenuStaff;

namespace SmartMenu.Services.MenuCommand
{
    public class MenuCommandService : IMenuCommandService
    {
        private readonly IMenuCommandRepository _menuCommandRepository;
        private readonly IMenuStaffRepository _menuStaffRepository;
        private readonly ILanguageRepository _languageRepository;

        public MenuCommandService(
            IMenuCommandRepository menuCommandRepository,
            IMenuStaffRepository menuStaffRepository,
            ILanguageRepository languageRepository)
        {
            _menuCommandRepository = menuCommandRepository;
            _menuStaffRepository = menuStaffRepository;
            _languageRepository = languageRepository;
        }

        public async Task<IEnumerable<MenuCommandListItemViewModel>> GetMenuCommandsAsync(int tenantId, int menuId)
        {
            var languages = (await _languageRepository.GetByTenantIdAsync(tenantId)).ToList();
            var menuCommands = await _menuCommandRepository.GetByMenuIdWithTextsAndStaffsAsync(menuId);
            var defaultLangId = languages.FirstOrDefault(l => l.IsDefault)?.Id;

            return menuCommands.Select(l =>
            {
                var defaultText =
                    l.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == defaultLangId)?.Text
                    ?? l.MenuCommandTexts.FirstOrDefault()?.Text
                    ?? FallbackText.NoText;

                return new MenuCommandListItemViewModel
                {
                    Id = l.Id,
                    Icon = l.Icon,
                    HasCustomerMessage = l.HasCustomerMessage,
                    DefaultText = defaultText,
                    SystemMessage = l.SystemMessage,
                    TextsByLanguage = languages.ToDictionary(
                        lang => lang.Name,
                        lang => l.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? FallbackText.NoText)
                };
            });
        }

        public async Task<CreateMenuCommandViewModel> GetCreateMenuCommandModelAsync(int tenantId, int menuId)
        {
            var languages = await GetLanguageViewModelsAsync(tenantId);
            var staff = (await _menuStaffRepository.GetByMenuIdAsync(menuId))
                .OrderBy(s => s.Name)
                .Select(s => new MenuStaffOptionViewModel { Id = s.Id, Name = s.Name, PhoneNumber = s.PhoneNumber })
                .ToList();

            return new CreateMenuCommandViewModel
            {
                MenuId = menuId,
                AvailableLanguages = languages.ToList(),
                AvailableStaff = staff,
                Texts = languages.Select(l => new MenuCommandTextViewModel { LanguageId = l.Id }).ToList()
            };
        }

        public async Task CreateMenuCommandAsync(CreateMenuCommandViewModel model)
        {
            var menuCommand = new Data.Entities.MenuCommand
            {
                MenuId = model.MenuId,
                Icon = model.Icon,
                HasCustomerMessage = model.HasCustomerMessage,
                SystemMessage = model.SystemMessage
            };
            await _menuCommandRepository.AddAsync(menuCommand);

            foreach (var text in model.Texts.Where(t => !string.IsNullOrWhiteSpace(t.Text)))
            {
                await _menuCommandRepository.AddTextAsync(new MenuCommandText
                {
                    MenuCommandId = menuCommand.Id,
                    LanguageId = text.LanguageId,
                    Text = text.Text
                });
            }

            if (model.SelectedStaffIds != null)
            {
                foreach (var staffId in model.SelectedStaffIds.Distinct())
                {
                    await _menuCommandRepository.AddStaffMappingAsync(new MenuCommandStaff
                    {
                        MenuCommandId = menuCommand.Id,
                        MenuStaffId = staffId
                    });
                }
            }
        }

        public async Task<EditMenuCommandViewModel?> GetEditMenuCommandModelAsync(int tenantId, int id)
        {
            var menuCommand = await _menuCommandRepository.GetByIdWithTextsAndStaffsAsync(id);
            if (menuCommand == null)
                return null;

            var languages = await GetLanguageViewModelsAsync(tenantId);
            var staff = (await _menuStaffRepository.GetByMenuIdAsync(menuCommand.MenuId))
                .OrderBy(s => s.Name)
                .Select(s => new MenuStaffOptionViewModel { Id = s.Id, Name = s.Name, PhoneNumber = s.PhoneNumber })
                .ToList();

            var texts = languages.Select(lang => new MenuCommandTextViewModel
            {
                LanguageId = lang.Id,
                Text = menuCommand.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == lang.Id)?.Text ?? ""
            }).ToList();

            return new EditMenuCommandViewModel
            {
                MenuCommandId = menuCommand.Id,
                Icon = menuCommand.Icon,
                HasCustomerMessage = menuCommand.HasCustomerMessage,
                SystemMessage = menuCommand.SystemMessage,
                AvailableLanguages = languages.ToList(),
                AvailableStaff = staff,
                SelectedStaffIds = menuCommand.MenuCommandStaffs.Select(cs => cs.MenuStaffId).ToList(),
                Texts = texts
            };
        }

        public async Task<bool> EditMenuCommandAsync(int id, EditMenuCommandViewModel model)
        {
            var menuCommand = await _menuCommandRepository.GetByIdWithTextsAndStaffsAsync(id);
            if (menuCommand == null)
                return false;

            menuCommand.Icon = model.Icon;
            menuCommand.HasCustomerMessage = model.HasCustomerMessage;
            menuCommand.SystemMessage = model.SystemMessage;

            foreach (var textVm in model.Texts)
            {
                var textEntity = menuCommand.MenuCommandTexts.FirstOrDefault(t => t.LanguageId == textVm.LanguageId);
                if (textEntity != null)
                    textEntity.Text = textVm.Text;
                else if (!string.IsNullOrWhiteSpace(textVm.Text))
                    await _menuCommandRepository.AddTextAsync(new MenuCommandText
                    {
                        MenuCommandId = menuCommand.Id,
                        LanguageId = textVm.LanguageId,
                        Text = textVm.Text
                    });
            }

            var currentStaffIds = menuCommand.MenuCommandStaffs.Select(s => s.MenuStaffId).ToList();
            var newStaffIds = model.SelectedStaffIds?.Distinct().ToList() ?? new List<int>();

            var toRemove = menuCommand.MenuCommandStaffs.Where(m => !newStaffIds.Contains(m.MenuStaffId)).ToList();
            if (toRemove.Any())
                await _menuCommandRepository.RemoveStaffMappingsRangeAsync(toRemove);

            foreach (var sid in newStaffIds.Where(sid => !currentStaffIds.Contains(sid)))
            {
                await _menuCommandRepository.AddStaffMappingAsync(new MenuCommandStaff
                {
                    MenuCommandId = menuCommand.Id,
                    MenuStaffId = sid
                });
            }

            await _menuCommandRepository.UpdateAsync(menuCommand);
            return true;
        }

        public async Task<bool> DeleteMenuCommandAsync(int id)
        {
            var menuCommand = await _menuCommandRepository.GetByIdWithTextsAndStaffsAsync(id);
            if (menuCommand == null)
                return false;

            await _menuCommandRepository.RemoveStaffMappingsRangeAsync(menuCommand.MenuCommandStaffs);
            await _menuCommandRepository.RemoveTextsRangeAsync(menuCommand.MenuCommandTexts);
            await _menuCommandRepository.DeleteAsync(menuCommand);
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
