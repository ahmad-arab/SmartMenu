using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;
using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.Category
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Data.Entities.Category>> GetByMenuIdWithTitlesAsync(int menuId)
        {
            return await _context.Categories
                .Where(c => c.MenuId == menuId)
                .Include(c => c.CategoryTitles)
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Data.Entities.Category>> GetByMenuIdWithTitlesIncludingLanguageAsync(int menuId)
        {
            return await _context.Categories
                .Where(c => c.MenuId == menuId)
                .Include(c => c.CategoryTitles).ThenInclude(ct => ct.Language)
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        public async Task<Data.Entities.Category?> GetByIdWithTitlesAndDescriptionsAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.CategoryTitles)
                .Include(c => c.CategoryDescriptions)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Data.Entities.Category?> GetByIdWithMenuTitlesAndLanguagesAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.CategoryTitles).ThenInclude(ct => ct.Language)
                .Include(c => c.Menu).ThenInclude(m => m.MenuTitles).ThenInclude(mt => mt.Language)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Data.Entities.Category>> GetByMenuIdForDeleteAsync(int menuId)
        {
            return await _context.Categories
                .Where(c => c.MenuId == menuId)
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        public async Task AddAsync(Data.Entities.Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task AddTitleAsync(CategoryTitle title)
        {
            _context.CategoryTitles.Add(title);
            await _context.SaveChangesAsync();
        }

        public async Task AddDescriptionAsync(CategoryDescription desc)
        {
            _context.CategoryDescriptions.Add(desc);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Data.Entities.Category category)
        {
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTitlesRangeAsync(IEnumerable<CategoryTitle> titles)
        {
            _context.CategoryTitles.RemoveRange(titles);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDescriptionsRangeAsync(IEnumerable<CategoryDescription> descs)
        {
            _context.CategoryDescriptions.RemoveRange(descs);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Data.Entities.Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
