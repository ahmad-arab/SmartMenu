using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;
using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.MenuLable
{
    public class MenuLableRepository : IMenuLableRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuLableRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Data.Entities.MenuLable>> GetByMenuIdWithTextsAsync(int menuId)
        {
            return await _context.MenuLables
                .Where(l => l.MenuId == menuId)
                .Include(l => l.MenuLableTexts)
                .ToListAsync();
        }

        public async Task<Data.Entities.MenuLable?> GetByIdWithTextsAsync(int id)
        {
            return await _context.MenuLables
                .Include(l => l.MenuLableTexts)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task AddAsync(Data.Entities.MenuLable menuLable)
        {
            _context.MenuLables.Add(menuLable);
            await _context.SaveChangesAsync();
        }

        public async Task AddTextAsync(MenuLableText text)
        {
            _context.MenuLableTexts.Add(text);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Data.Entities.MenuLable menuLable)
        {
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTextsRangeAsync(IEnumerable<MenuLableText> texts)
        {
            _context.MenuLableTexts.RemoveRange(texts);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Data.Entities.MenuLable menuLable)
        {
            _context.MenuLables.Remove(menuLable);
            await _context.SaveChangesAsync();
        }
    }
}
