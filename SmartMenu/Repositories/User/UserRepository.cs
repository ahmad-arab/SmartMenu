using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;
using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApplicationUser>> GetByTenantIdAsync(int tenantId)
        {
            return await _context.ApplicationUsers
                .Where(u => u.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _context.ApplicationUsers.FindAsync(id);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.ApplicationUsers
                .AnyAsync(u => u.UserName == username);
        }

        public async Task AddUserRoleAsync(ApplicationUserRole role)
        {
            _context.UserRoles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            _context.ApplicationUsers.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
