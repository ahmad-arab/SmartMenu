using SmartMenu.Models.Language;

namespace SmartMenu.Services.Language
{
    public interface ILanguageService
    {
        Task<IEnumerable<LanguageListItemViewModel>> GetLanguagesAsync(int tenantId);
        Task CreateLanguageAsync(int tenantId, CreateLanguageViewModel model);
        Task<EditLanguageViewModel?> GetLanguageForEditAsync(int id);
        Task<bool> EditLanguageAsync(int id, EditLanguageViewModel model);
        Task<bool> DeleteLanguageAsync(int id);
    }
}
