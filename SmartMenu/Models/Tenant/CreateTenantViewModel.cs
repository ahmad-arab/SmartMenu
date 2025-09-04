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

        [Display(Name = "Logo")]
        public IFormFile Logo { get; set; }
    }
}
