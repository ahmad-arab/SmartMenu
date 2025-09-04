using Microsoft.AspNetCore.Http;
using SmartMenu.Models.Category;
using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.Menu
{
    public class EditMenuViewModel
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Current Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "New Image")]
        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }

        public List<MenuTitleViewModel> Titles { get; set; } = new();
        public List<LanguageListItemViewModel> AvailableLanguages { get; set; } = new();
    }
}