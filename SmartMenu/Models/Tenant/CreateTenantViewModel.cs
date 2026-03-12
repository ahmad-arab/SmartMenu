using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SmartMenu.Models.Tenant
{
    public class CreateTenantViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [Display(Name = "Tenant Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Allowed Menus Count")]
        public int AllowedMenusCount { get; set; } = 1;

        [Required]
        [Display(Name = "Use Commands")]
        public bool UseCommands { get; set; } = false;

        [Display(Name = "Logo")]
        public IFormFile Logo { get; set; }
    }
}
