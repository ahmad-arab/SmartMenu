using SmartMenu.Models.Item;
using SmartMenu.Models.Language;

namespace SmartMenu.Services.Item
{
    public interface IItemService
    {
        Task<CreateItemViewModel> GetCreateItemModelAsync(int tenantId, int categoryId);
        Task CreateItemAsync(CreateItemViewModel model);
        Task<EditItemViewModel?> GetEditItemModelAsync(int tenantId, int id);
        Task<bool> EditItemAsync(EditItemViewModel model);
        Task<bool> DeleteItemAsync(int id);
        Task<IEnumerable<LanguageListItemViewModel>> GetLanguagesForTenantAsync(int tenantId);
    }
}
