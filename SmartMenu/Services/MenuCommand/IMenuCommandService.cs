using SmartMenu.Models.MenuCommand;

namespace SmartMenu.Services.MenuCommand
{
    public interface IMenuCommandService
    {
        Task<IEnumerable<MenuCommandListItemViewModel>> GetMenuCommandsAsync(int tenantId, int menuId);
        Task<CreateMenuCommandViewModel> GetCreateMenuCommandModelAsync(int tenantId, int menuId);
        Task CreateMenuCommandAsync(CreateMenuCommandViewModel model);
        Task<EditMenuCommandViewModel?> GetEditMenuCommandModelAsync(int tenantId, int id);
        Task<bool> EditMenuCommandAsync(int id, EditMenuCommandViewModel model);
        Task<bool> DeleteMenuCommandAsync(int id);
    }
}
