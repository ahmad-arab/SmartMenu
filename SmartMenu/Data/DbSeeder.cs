using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SmartMenu.Data
{
    public class DbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public DbSeeder(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Applies any pending migrations and seeds initial data.
        /// </summary>
        public void ApplyMigrationsAndSeed()
        {
            _context.Database.Migrate();
            SeedData();
        }

        /// <summary>
        /// Seeds initial data into the database.
        /// </summary>
        private void SeedData()
        {
            // Read defaults from configuration
            var adminUsername = _configuration["AdminDefaults:Username"];
            var adminPassword = _configuration["AdminDefaults:Password"];

            // Ensure "Admin" role exists
            var adminRole = _context.ApplicationRoles.FirstOrDefault(r => r.Name == "Admin");
            if (adminRole == null)
            {
                adminRole = new Entities.ApplicationRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                };
                _context.ApplicationRoles.Add(adminRole);
                _context.SaveChanges();
            }

            // Ensure "TenantAdmin" role exists
            var tenantAdminRole = _context.ApplicationRoles.FirstOrDefault(r => r.Name == "TenantAdmin");
            if (tenantAdminRole == null)
            {
                tenantAdminRole = new Entities.ApplicationRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "TenantAdmin",
                    NormalizedName = "TENANTADMIN"
                };
                _context.ApplicationRoles.Add(tenantAdminRole);
                _context.SaveChanges();
            }

            // Ensure root admin user exists
            var rootAdminUser = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == adminUsername);
            if (rootAdminUser == null)
            {
                rootAdminUser = new Entities.ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = adminUsername,
                    NormalizedUserName = adminUsername.ToUpperInvariant(),
                    Email = "rootadmin@example.com",
                    NormalizedEmail = "ROOTADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Entities.ApplicationUser>();
                rootAdminUser.PasswordHash = hasher.HashPassword(rootAdminUser, adminPassword);

                _context.ApplicationUsers.Add(rootAdminUser);
                _context.SaveChanges();
            }

            // Ensure root admin is in "Admin" role
            var userRoleExists = _context.ApplicationUserRoles
                .Any(ur => ur.UserId == rootAdminUser.Id && ur.RoleId == adminRole.Id);
            if (!userRoleExists)
            {
                var userRole = new Entities.ApplicationUserRole
                {
                    UserId = rootAdminUser.Id,
                    RoleId = adminRole.Id
                };
                _context.ApplicationUserRoles.Add(userRole);
                _context.SaveChanges();
            }
        }
    }
}
