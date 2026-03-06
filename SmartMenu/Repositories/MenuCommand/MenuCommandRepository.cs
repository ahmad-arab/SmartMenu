using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;
using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.MenuCommand
{
    public class MenuCommandRepository : IMenuCommandRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuCommandRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Data.Entities.MenuCommand>> GetByMenuIdWithTextsAndStaffsAsync(int menuId)
        {
            return await _context.MenuCommands
                .Where(c => c.MenuId == menuId)
                .Include(c => c.MenuCommandTexts)
                .Include(c => c.MenuCommandStaffs)
                .ToListAsync();
        }

        public async Task<Data.Entities.MenuCommand?> GetByIdWithTextsAndStaffsAsync(int id)
        {
            return await _context.MenuCommands
                .Include(c => c.MenuCommandTexts)
                .Include(c => c.MenuCommandStaffs)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Data.Entities.MenuCommand?> GetByIdForSendAsync(int id)
        {
            return await _context.MenuCommands
                .Include(mc => mc.Menu)
                .Include(mc => mc.MenuCommandStaffs).ThenInclude(mcs => mcs.MenuStaff)
                .FirstOrDefaultAsync(mc => mc.Id == id);
        }

        public async Task AddAsync(Data.Entities.MenuCommand command)
        {
            _context.MenuCommands.Add(command);
            await _context.SaveChangesAsync();
        }

        public async Task AddTextAsync(MenuCommandText text)
        {
            _context.MenuCommandTexts.Add(text);
            await _context.SaveChangesAsync();
        }

        public async Task AddStaffMappingAsync(MenuCommandStaff mapping)
        {
            _context.MenuCommandStaffs.Add(mapping);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Data.Entities.MenuCommand command)
        {
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTextsRangeAsync(IEnumerable<MenuCommandText> texts)
        {
            _context.MenuCommandTexts.RemoveRange(texts);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveStaffMappingsRangeAsync(IEnumerable<MenuCommandStaff> mappings)
        {
            _context.MenuCommandStaffs.RemoveRange(mappings);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Data.Entities.MenuCommand command)
        {
            _context.MenuCommands.Remove(command);
            await _context.SaveChangesAsync();
        }
    }
}
