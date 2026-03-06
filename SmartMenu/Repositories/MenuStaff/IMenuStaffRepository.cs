using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.MenuStaff
{
    public interface IMenuStaffRepository
    {
        Task<IEnumerable<Data.Entities.MenuStaff>> GetByMenuIdAsync(int menuId);
        Task<Data.Entities.MenuStaff?> GetByIdAsync(int id);
        Task<Data.Entities.MenuStaff?> GetByIdWithTimeSlotsAsync(int id);
        Task<Data.Entities.MenuStaff?> GetByIdWithAllIncludesAsync(int id);
        Task<Data.Entities.MenuStaff?> GetByIdWithMenuTitlesAsync(int id);
        Task<IEnumerable<Data.Entities.MenuStaff>> GetByMenuIdWithTimeSlotsAsync(int menuId, IEnumerable<int> staffIds);
        Task<IEnumerable<MenuStaffTimeSlot>> GetTimeSlotsByStaffIdsAndDayAsync(IEnumerable<int> staffIds, DayOfWeek day);
        Task AddAsync(Data.Entities.MenuStaff staff);
        Task AddTimeSlotAsync(MenuStaffTimeSlot slot);
        Task UpdateAsync(Data.Entities.MenuStaff staff);
        Task RemoveTimeSlotsRangeAsync(IEnumerable<MenuStaffTimeSlot> slots);
        Task RemoveCommandStaffMappingsRangeAsync(IEnumerable<MenuCommandStaff> mappings);
        Task DeleteAsync(Data.Entities.MenuStaff staff);
    }
}
