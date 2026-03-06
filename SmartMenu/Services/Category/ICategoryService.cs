using SmartMenu.Models.Category;
using SmartMenu.Models.Language;

namespace SmartMenu.Services.Category
{
    public interface ICategoryService
    {
        Task<CreateCategoryViewModel> GetCreateCategoryModelAsync(int tenantId, int menuId);
        Task CreateCategoryAsync(CreateCategoryViewModel model);
        Task<EditCategoryViewModel?> GetEditCategoryModelAsync(int tenantId, int id);
        Task<bool> EditCategoryAsync(EditCategoryViewModel model);
        Task<CategoryViewModel?> GetCategoryPageModelAsync(int tenantId, int id);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<LanguageListItemViewModel>> GetLanguagesForTenantAsync(int tenantId);
    }
}
