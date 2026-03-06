using SmartMenu.Models.User;

namespace SmartMenu.Services.User
{
    public interface IUserService
    {
        Task<IEnumerable<UserListViewModel>> GetUsersByTenantAsync(int tenantId);
        Task<(bool Success, string Message)> CreateTenantUserAsync(CreateUserViewModel model);
        Task<(bool Success, string Message)> ChangePasswordAsync(ChangeUserPasswordViewModel model);
        Task<(bool Success, string Message)> DeleteUserAsync(string id);
    }
}
