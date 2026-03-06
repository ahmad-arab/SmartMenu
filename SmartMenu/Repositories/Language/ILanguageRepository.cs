using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.Language
{
    public interface ILanguageRepository
    {
        Task<IEnumerable<Data.Entities.Language>> GetByTenantIdAsync(int tenantId);
        Task<Data.Entities.Language?> GetByIdAsync(int id);
        Task<Data.Entities.Language?> GetByIdWithAllRelatedAsync(int id);
        Task AddAsync(Data.Entities.Language language);
        Task UpdateAsync(Data.Entities.Language language);
        Task DeleteAsync(Data.Entities.Language language);
    }
}
