using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class Tenant
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public int AllowedMenusCount { get; set; } = 1;
        public bool UseCommands { get; set; } = false;

        public ICollection<ApplicationUser> TenantUsers { get; set; } = new List<ApplicationUser>();
    }
}
