using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.MenuLable
{
    public interface IMenuLableRepository
    {
        Task<IEnumerable<Data.Entities.MenuLable>> GetByMenuIdWithTextsAsync(int menuId);
        Task<Data.Entities.MenuLable?> GetByIdWithTextsAsync(int id);
        Task AddAsync(Data.Entities.MenuLable menuLable);
        Task AddTextAsync(MenuLableText text);
        Task UpdateAsync(Data.Entities.MenuLable menuLable);
        Task RemoveTextsRangeAsync(IEnumerable<MenuLableText> texts);
        Task DeleteAsync(Data.Entities.MenuLable menuLable);
    }
}
