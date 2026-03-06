using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;
using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.Menu
{
    public class MenuRepository : IMenuRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Data.Entities.Menu>> GetByTenantIdWithTitlesAsync(int tenantId)
        {
            return await _context.Menus
                .Where(m => m.TenantId == tenantId)
                .Include(m => m.MenuTitles)
                .ToListAsync();
        }

        public async Task<Data.Entities.Menu?> GetByIdAsync(int id)
        {
            return await _context.Menus.FindAsync(id);
        }

        public async Task<Data.Entities.Menu?> GetByIdWithTitlesAsync(int id, int tenantId)
        {
            return await _context.Menus
                .Include(m => m.MenuTitles)
                .Where(m => m.Id == id && m.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<Data.Entities.Menu?> GetByIdForDeleteAsync(int id, int tenantId)
        {
            return await _context.Menus
                .Include(m => m.MenuTitles)
                .Include(m => m.Categorys)
                .Include(m => m.MenuLables)
                .Include(m => m.MenuCommands)
                .Include(m => m.MenuStaffs)
                .FirstOrDefaultAsync(m => m.Id == id && m.TenantId == tenantId);
        }

        public async Task<Data.Entities.Menu?> GetByIdForPublicViewAsync(int id)
        {
            return await _context.Menus
                .Include(m => m.MenuTitles).ThenInclude(mt => mt.Language)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Data.Entities.Menu menu)
        {
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
        }

        public async Task AddTitleAsync(MenuTitle title)
        {
            _context.MenuTitles.Add(title);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Data.Entities.Menu menu)
        {
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTitlesRangeAsync(IEnumerable<MenuTitle> titles)
        {
            _context.MenuTitles.RemoveRange(titles);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Data.Entities.Menu menu)
        {
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
        }
    }
}
