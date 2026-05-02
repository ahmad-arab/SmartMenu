using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.Menu
{
    public interface IMenuRepository
    {
        Task<IEnumerable<Data.Entities.Menu>> GetByTenantIdWithTitlesAsync(int tenantId);
        Task<int> GetCountByTenantIdAsync(int tenantId);
        Task<Data.Entities.Menu?> GetByIdAsync(int id);
        Task<Data.Entities.Menu?> GetByIdWithTitlesAsync(int id, int tenantId);
        Task<Data.Entities.Menu?> GetByIdForDeleteAsync(int id, int tenantId);
        Task<Data.Entities.Menu?> GetByIdForPublicViewAsync(int id);
        Task AddAsync(Data.Entities.Menu menu);
        Task AddTitleAsync(MenuTitle title);
        Task AddHeroSubtitleTextAsync(MenuHeroSubtitleText text);
        Task AddCategoryIndexTitleTextAsync(MenuCategoryIndexTitleText text);
        Task UpdateAsync(Data.Entities.Menu menu);
        Task RemoveTitlesRangeAsync(IEnumerable<MenuTitle> titles);
        Task RemoveHeroSubtitleTextsRangeAsync(IEnumerable<MenuHeroSubtitleText> texts);
        Task RemoveCategoryIndexTitleTextsRangeAsync(IEnumerable<MenuCategoryIndexTitleText> texts);
        Task DeleteAsync(Data.Entities.Menu menu);
    }
}
