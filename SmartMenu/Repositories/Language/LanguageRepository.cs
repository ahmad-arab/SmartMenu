using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;

namespace SmartMenu.Repositories.Language
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly ApplicationDbContext _context;

        public LanguageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Data.Entities.Language>> GetByTenantIdAsync(int tenantId)
        {
            return await _context.Languages
                .Where(l => l.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<Data.Entities.Language?> GetByIdAsync(int id)
        {
            return await _context.Languages.FindAsync(id);
        }

        public async Task<Data.Entities.Language?> GetByIdWithAllRelatedAsync(int id)
        {
            return await _context.Languages
                .Include(l => l.LanguageTexts)
                .Include(l => l.CategoryTitles)
                .Include(l => l.CategoryDescriptions)
                .Include(l => l.ItemTitles)
                .Include(l => l.ItemDescriptions)
                .Include(l => l.MenuLanguages)
                .Include(l => l.MenuLableTexts)
                .Include(l => l.MenuCommandTexts)
                .Include(l => l.MenuTitles)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task AddAsync(Data.Entities.Language language)
        {
            _context.Languages.Add(language);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Data.Entities.Language language)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Data.Entities.Language language)
        {
            _context.Languages.Remove(language);
            await _context.SaveChangesAsync();
        }
    }
}
