using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.MenuCommand
{
    public interface IMenuCommandRepository
    {
        Task<IEnumerable<Data.Entities.MenuCommand>> GetByMenuIdWithTextsAndStaffsAsync(int menuId);
        Task<Data.Entities.MenuCommand?> GetByIdWithTextsAndStaffsAsync(int id);
        Task<Data.Entities.MenuCommand?> GetByIdForSendAsync(int id);
        Task AddAsync(Data.Entities.MenuCommand command);
        Task AddTextAsync(MenuCommandText text);
        Task AddStaffMappingAsync(MenuCommandStaff mapping);
        Task UpdateAsync(Data.Entities.MenuCommand command);
        Task RemoveTextsRangeAsync(IEnumerable<MenuCommandText> texts);
        Task RemoveStaffMappingsRangeAsync(IEnumerable<MenuCommandStaff> mappings);
        Task DeleteAsync(Data.Entities.MenuCommand command);
    }
}
