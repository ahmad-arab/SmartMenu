using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;
using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.MenuStaff
{
    public class MenuStaffRepository : IMenuStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuStaffRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Data.Entities.MenuStaff>> GetByMenuIdAsync(int menuId)
        {
            return await _context.MenuStaffs
                .Where(s => s.MenuId == menuId)
                .ToListAsync();
        }

        public async Task<Data.Entities.MenuStaff?> GetByIdAsync(int id)
        {
            return await _context.MenuStaffs.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Data.Entities.MenuStaff?> GetByIdWithTimeSlotsAsync(int id)
        {
            return await _context.MenuStaffs
                .Include(s => s.TimeSlots)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Data.Entities.MenuStaff?> GetByIdWithAllIncludesAsync(int id)
        {
            return await _context.MenuStaffs
                .Include(s => s.TimeSlots)
                .Include(s => s.MenuCommandStaffs)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Data.Entities.MenuStaff?> GetByIdWithMenuTitlesAsync(int id)
        {
            return await _context.MenuStaffs
                .Include(s => s.Menu).ThenInclude(m => m.MenuTitles)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Data.Entities.MenuStaff>> GetByMenuIdWithTimeSlotsAsync(int menuId, IEnumerable<int> staffIds)
        {
            return await _context.MenuStaffs
                .Include(s => s.TimeSlots)
                .Where(s => s.MenuId == menuId && staffIds.Contains(s.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuStaffTimeSlot>> GetTimeSlotsByStaffIdsAndDayAsync(IEnumerable<int> staffIds, DayOfWeek day)
        {
            return await _context.MenuStaffTimeSlots
                .Where(ts => staffIds.Contains(ts.MenuStaffId) && ts.DayOfWeek == day)
                .ToListAsync();
        }

        public async Task AddAsync(Data.Entities.MenuStaff staff)
        {
            _context.MenuStaffs.Add(staff);
            await _context.SaveChangesAsync();
        }

        public async Task AddTimeSlotAsync(MenuStaffTimeSlot slot)
        {
            _context.MenuStaffTimeSlots.Add(slot);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Data.Entities.MenuStaff staff)
        {
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTimeSlotsRangeAsync(IEnumerable<MenuStaffTimeSlot> slots)
        {
            _context.MenuStaffTimeSlots.RemoveRange(slots);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCommandStaffMappingsRangeAsync(IEnumerable<MenuCommandStaff> mappings)
        {
            _context.MenuCommandStaffs.RemoveRange(mappings);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Data.Entities.MenuStaff staff)
        {
            _context.MenuStaffs.Remove(staff);
            await _context.SaveChangesAsync();
        }
    }
}
