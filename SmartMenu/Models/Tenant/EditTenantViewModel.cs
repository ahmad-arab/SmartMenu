using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SmartMenu.Models.Tenant
{
    public class EditTenantViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [Display(Name = "Tenant Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Allowed Menus Count")]
        public int AllowedMenusCount { get; set; }

        [Required]
        [Display(Name = "Use Commands")]
        public bool UseCommands { get; set; }

        [Display(Name = "Current Logo")]
        public string? LogoUrl { get; set; }

        [Display(Name = "Change Logo")]
        public IFormFile? Logo { get; set; }
    }
}
