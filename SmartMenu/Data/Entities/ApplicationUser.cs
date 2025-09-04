using Microsoft.AspNetCore.Identity;

namespace SmartMenu.Data.Entities
{
    public class ApplicationUser : IdentityUser<string>
    {
        public int? TenantId { get; set; }
        public virtual Tenant? Tenant { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    }
}
