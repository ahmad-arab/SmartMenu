using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.Language
{
    public class EditLanguageViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name must be at most 100 characters.")]
        public string Name { get; set; }

        [Display(Name = "Is Default Language")]
        public bool IsDefault { get; set; } = false;

        [Display(Name = "Is Right To Left")]
        public bool IsRtl { get; set; }
    }
}