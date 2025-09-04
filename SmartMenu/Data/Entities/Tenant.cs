using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class Tenant
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }

        public ICollection<ApplicationUser> TenantUsers { get; set; } = new List<ApplicationUser>();
    }
}
