using SmartMenu.Data.Enums;
using SmartMenu.Models.Category;
using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.MenuLable
{
    public class CreateMenuLableViewModel
    {
        [Required(ErrorMessage = "Menu is required.")]
        public int MenuId { get; set; }

        [Required(ErrorMessage = "Icon is required.")]
        public IconIdentifier Icon { get; set; }

        [MinLength(1, ErrorMessage = "At least one text entry is required.")]
        public List<MenuLableTextViewModel> Texts { get; set; } = new();

        public List<LanguageListItemViewModel> AvailableLanguages { get; set; } = new();
    }

    public class MenuLableTextViewModel
    {
        public int LanguageId { get; set; }

        [Required(ErrorMessage = "Text is required.")]
        [StringLength(1000, ErrorMessage = "Text cannot be longer than 1000 characters.")]
        public string Text { get; set; }
    }
}
