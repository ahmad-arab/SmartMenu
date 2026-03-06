using SmartMenu.Models.MenuLable;

namespace SmartMenu.Services.MenuLable
{
    public interface IMenuLableService
    {
        Task<IEnumerable<MenuLableListItemViewModel>> GetMenuLablesAsync(int tenantId, int menuId);
        Task<CreateMenuLableViewModel> GetCreateMenuLableModelAsync(int tenantId, int menuId);
        Task CreateMenuLableAsync(CreateMenuLableViewModel model);
        Task<EditMenuLableViewModel?> GetEditMenuLableModelAsync(int tenantId, int id);
        Task<bool> EditMenuLableAsync(int id, EditMenuLableViewModel model);
        Task<bool> DeleteMenuLableAsync(int id);
    }
}
