using Microsoft.AspNetCore.Mvc.Rendering;
using SmartMenu.Models.MenuStaff;
using SmartMenu.Models.TimeTable;

namespace SmartMenu.Services.MenuStaff
{
    public interface IMenuStaffService
    {
        Task<IEnumerable<MenuStaffListItemViewModel>> GetMenuStaffsAsync(int menuId);
        Task<CreateMenuStaffViewModel> GetCreateMenuStaffModelAsync(int menuId);
        Task CreateMenuStaffAsync(CreateMenuStaffViewModel model);
        Task<EditMenuStaffViewModel?> GetEditMenuStaffModelAsync(int id);
        Task<bool> EditMenuStaffAsync(int id, EditMenuStaffViewModel model);
        Task<bool> DeleteMenuStaffAsync(int id);
        Task<(bool Success, string Message)> RegisterMenuStaffAsync(int id, int tenantId);
        Task<StaffTimeTableViewModel?> GetStaffTimeTableModelAsync(int id);
        Task<bool> EditStaffTimeTableAsync(StaffTimeTableViewModel model);
        Task<BulkStaffTimeTableViewModel> GetBulkEditModelAsync(int menuId);
        Task<bool> BulkEditStaffTimeTableAsync(BulkStaffTimeTableViewModel model);
    }
}
