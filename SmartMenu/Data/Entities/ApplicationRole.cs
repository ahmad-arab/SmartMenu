using Microsoft.AspNetCore.Identity;

namespace SmartMenu.Data.Entities
{
    public class ApplicationRole : IdentityRole<string>
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    }
}
