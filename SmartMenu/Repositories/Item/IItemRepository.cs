using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.Item
{
    public interface IItemRepository
    {
        Task<IEnumerable<Data.Entities.Item>> GetByCategoryIdWithTitlesAsync(int categoryId);
        Task<IEnumerable<Data.Entities.Item>> GetAvailableItemsByCategoryIdsAsync(IEnumerable<int> categoryIds);
        Task<IEnumerable<Data.Entities.Item>> GetAvailableItemsByCategoryIdAsync(int categoryId);
        Task<Data.Entities.Item?> GetByIdWithTitlesAndDescriptionsAsync(int id);
        Task AddAsync(Data.Entities.Item item);
        Task AddTitleAsync(ItemTitle title);
        Task AddDescriptionAsync(ItemDescription desc);
        Task UpdateAsync(Data.Entities.Item item);
        Task RemoveTitlesRangeAsync(IEnumerable<ItemTitle> titles);
        Task RemoveDescriptionsRangeAsync(IEnumerable<ItemDescription> descs);
        Task DeleteAsync(Data.Entities.Item item);
    }
}
