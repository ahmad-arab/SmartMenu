using Microsoft.AspNetCore.Identity;

namespace SmartMenu.Data.Entities
{
    public class ApplicationUserRole:IdentityUserRole<string>
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string RoleId { get; set; }
        public ApplicationRole Role { get; set; }
    }
}
