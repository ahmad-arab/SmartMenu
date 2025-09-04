using Microsoft.AspNetCore.Http;
using SmartMenu.Models.Category;
using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.Menu
{
    public class CreateMenuViewModel
    {
        [Display(Name = "Menu Image")]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        public List<MenuTitleViewModel> Titles { get; set; } = new();
        public List<LanguageListItemViewModel> AvailableLanguages { get; set; } = new();
    }

    public class MenuTitleViewModel
    {
        public int LanguageId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Text { get; set; }
    }
}