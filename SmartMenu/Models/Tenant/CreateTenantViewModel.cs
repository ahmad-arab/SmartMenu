using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SmartMenu.Attributes.Validation;

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

        [Display(Name = "Domain Name")]
        [DomainName(ErrorMessage = "Please enter a valid domain or subdomain name (for example: example.com or menu.example.com).")]
        public string? DomainName { get; set; }

        [Display(Name = "Landing Page URL")]
        [UrlOrRelativePath(ErrorMessage = "Please enter a valid URL.")]
        public string? LandingPageUrl { get; set; }

        [Display(Name = "Logo")]
        public IFormFile Logo { get; set; }
    }
}
