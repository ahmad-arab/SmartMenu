using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;
using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.Item
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Data.Entities.Item>> GetByCategoryIdWithTitlesAsync(int categoryId)
        {
            return await _context.Items
                .Where(i => i.CategoryId == categoryId)
                .Include(i => i.ItemTitles)
                .OrderBy(i => i.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Data.Entities.Item>> GetAvailableItemsByCategoryIdsAsync(IEnumerable<int> categoryIds)
        {
            return await _context.Items
                .Include(i => i.ItemTitles).ThenInclude(it => it.Language)
                .Include(i => i.ItemDescriptions).ThenInclude(id => id.Language)
                .Where(i => categoryIds.Contains(i.CategoryId) && i.IsAvailable)
                .OrderBy(i => i.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Data.Entities.Item>> GetAvailableItemsByCategoryIdAsync(int categoryId)
        {
            return await _context.Items
                .Include(i => i.ItemTitles).ThenInclude(it => it.Language)
                .Include(i => i.ItemDescriptions).ThenInclude(id => id.Language)
                .Where(i => i.CategoryId == categoryId && i.IsAvailable)
                .OrderBy(i => i.Order)
                .ToListAsync();
        }

        public async Task<Data.Entities.Item?> GetByIdWithTitlesAndDescriptionsAsync(int id)
        {
            return await _context.Items
                .Include(i => i.ItemTitles)
                .Include(i => i.ItemDescriptions)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddAsync(Data.Entities.Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task AddTitleAsync(ItemTitle title)
        {
            _context.ItemTitles.Add(title);
            await _context.SaveChangesAsync();
        }

        public async Task AddDescriptionAsync(ItemDescription desc)
        {
            _context.ItemDescriptions.Add(desc);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Data.Entities.Item item)
        {
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTitlesRangeAsync(IEnumerable<ItemTitle> titles)
        {
            _context.ItemTitles.RemoveRange(titles);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDescriptionsRangeAsync(IEnumerable<ItemDescription> descs)
        {
            _context.ItemDescriptions.RemoveRange(descs);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Data.Entities.Item item)
        {
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
