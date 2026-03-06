using SmartMenu.Data.Entities;
using SmartMenu.Models.Language;
using SmartMenu.Repositories.Language;

namespace SmartMenu.Services.Language
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;

        public LanguageService(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }

        public async Task<IEnumerable<LanguageListItemViewModel>> GetLanguagesAsync(int tenantId)
        {
            var languages = await _languageRepository.GetByTenantIdAsync(tenantId);
            return languages.Select(l => new LanguageListItemViewModel
            {
                Id = l.Id,
                Name = l.Name,
                IsDefault = l.IsDefault,
                IsRtl = l.IsRtl
            });
        }

        public async Task CreateLanguageAsync(int tenantId, CreateLanguageViewModel model)
        {
            var language = new Data.Entities.Language
            {
                Name = model.Name,
                IsRtl = model.IsRtl,
                IsDefault = model.IsDefault,
                TenantId = tenantId
            };
            await _languageRepository.AddAsync(language);
        }

        public async Task<EditLanguageViewModel?> GetLanguageForEditAsync(int id)
        {
            var language = await _languageRepository.GetByIdAsync(id);
            if (language == null)
                return null;

            return new EditLanguageViewModel
            {
                Id = language.Id,
                Name = language.Name,
                IsRtl = language.IsRtl,
                IsDefault = language.IsDefault
            };
        }

        public async Task<bool> EditLanguageAsync(int id, EditLanguageViewModel model)
        {
            var language = await _languageRepository.GetByIdAsync(id);
            if (language == null)
                return false;

            language.Name = model.Name;
            language.IsRtl = model.IsRtl;
            language.IsDefault = model.IsDefault;

            await _languageRepository.UpdateAsync(language);
            return true;
        }

        public async Task<bool> DeleteLanguageAsync(int id)
        {
            var language = await _languageRepository.GetByIdWithAllRelatedAsync(id);
            if (language == null)
                return false;

            await _languageRepository.DeleteAsync(language);
            return true;
        }
    }
}
