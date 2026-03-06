using Microsoft.AspNetCore.Identity;
using SmartMenu.Data.Entities;
using SmartMenu.Models.User;
using SmartMenu.Repositories.User;

namespace SmartMenu.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserService(
            IUserRepository userRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<UserListViewModel>> GetUsersByTenantAsync(int tenantId)
        {
            var users = await _userRepository.GetByTenantIdAsync(tenantId);
            return users.Select(u => new UserListViewModel
            {
                Id = u.Id,
                Username = u.UserName
            });
        }

        public async Task<(bool Success, string Message)> CreateTenantUserAsync(CreateUserViewModel model)
        {
            if (await _userRepository.ExistsByUsernameAsync(model.Username))
                return (false, "Username already exists.");

            var appUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Email = model.Username,
                EmailConfirmed = true,
                TenantId = model.TenantId
            };

            var result = await _userManager.CreateAsync(appUser, model.Password);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            var tenantAdminRole = await _roleManager.FindByNameAsync("TenantAdmin");
            if (tenantAdminRole == null)
                return (false, "Role 'TenantAdmin' not found");

            await _userRepository.AddUserRoleAsync(new ApplicationUserRole
            {
                UserId = appUser.Id,
                RoleId = tenantAdminRole.Id
            });

            return (true, "User created and added to TenantAdmin role.");
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(ChangeUserPasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return (false, "User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            return (true, "Password changed successfully.");
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, "User not found.");

            await _userManager.DeleteAsync(user);
            return (true, "User deleted successfully.");
        }
    }
}
