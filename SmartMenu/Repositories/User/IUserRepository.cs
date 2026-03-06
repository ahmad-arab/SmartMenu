using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.User
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetByTenantIdAsync(int tenantId);
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<bool> ExistsByUsernameAsync(string username);
        Task AddUserRoleAsync(ApplicationUserRole role);
        Task DeleteAsync(ApplicationUser user);
    }
}
