using SmartMenu.Models.Language;
using SmartMenu.Models.Menu;
using SmartMenu.Models.Theme;

namespace SmartMenu.Services.Menu
{
    public interface IMenuService
    {
        Task<MenuListViewModel> GetMenuListAsync(int tenantId);
        Task<CreateMenuViewModel> GetCreateMenuModelAsync(int tenantId);
        Task CreateMenuAsync(int tenantId, CreateMenuViewModel model);
        Task<EditMenuViewModel?> GetEditMenuModelAsync(int tenantId, int id);
        Task<bool> EditMenuAsync(int tenantId, int id, EditMenuViewModel model);
        Task<Models.Menu.MenuViewModel?> GetMenuPageModelAsync(int tenantId, int id);
        Task<bool> DeleteMenuAsync(int tenantId, int id);
        Task<ThemeDesignerViewModel?> GetThemeDesignerModelAsync(int tenantId, int menuId, string previewUrl);
        Task<Data.Entities.Menu?> GetMenuForThemeEditAsync(int tenantId, int menuId);
        Task<bool> SaveThemeAsync(int tenantId, SaveThemeDto dto);
        Task<bool> MenuExistsForTenantAsync(int tenantId, int menuId);
    }
}
