using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.Tenant
{
    public class TenantModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AllowedMenusCount { get; set; }
        public bool UseCommands { get; set; }
        public string? LogoUrl { get; set; }
        public IFormFile? Logo { get; set; }
    }
}
