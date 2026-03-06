using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.Category
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Data.Entities.Category>> GetByMenuIdWithTitlesAsync(int menuId);
        Task<IEnumerable<Data.Entities.Category>> GetByMenuIdWithTitlesIncludingLanguageAsync(int menuId);
        Task<Data.Entities.Category?> GetByIdWithTitlesAndDescriptionsAsync(int id);
        Task<Data.Entities.Category?> GetByIdWithMenuTitlesAndLanguagesAsync(int id);
        Task<IEnumerable<Data.Entities.Category>> GetByMenuIdForDeleteAsync(int menuId);
        Task AddAsync(Data.Entities.Category category);
        Task AddTitleAsync(CategoryTitle title);
        Task AddDescriptionAsync(CategoryDescription desc);
        Task UpdateAsync(Data.Entities.Category category);
        Task RemoveTitlesRangeAsync(IEnumerable<CategoryTitle> titles);
        Task RemoveDescriptionsRangeAsync(IEnumerable<CategoryDescription> descs);
        Task DeleteAsync(Data.Entities.Category category);
    }
}
