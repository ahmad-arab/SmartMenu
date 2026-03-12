using Microsoft.EntityFrameworkCore;
using SmartMenu.Data;

namespace SmartMenu.Repositories.Tenant
{
    public class TenantRepository : ITenantRepository
    {
        private readonly ApplicationDbContext _context;

        public TenantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Data.Entities.Tenant>> GetAllAsync()
        {
            return await _context.Tenants.ToListAsync();
        }

        public async Task<Data.Entities.Tenant?> GetByIdAsync(int id)
        {
            return await _context.Tenants.FindAsync(id);
        }

        public async Task<Data.Entities.Tenant?> GetByIdNoTrackingAsync(int id)
        {
            return await _context.Tenants.AsNoTracking().FirstOrDefaultAsync(tenant => tenant.Id == id);
        }

        public async Task AddAsync(Data.Entities.Tenant tenant)
        {
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Data.Entities.Tenant tenant)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Data.Entities.Tenant tenant)
        {
            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
        }
    }
}
