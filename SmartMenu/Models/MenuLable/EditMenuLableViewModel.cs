using SmartMenu.Data.Enums;
using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.MenuLable
{
    public class EditMenuLableViewModel
    {
        [Required(ErrorMessage = "Menu label ID is required.")]
        public int MenuLableId { get; set; }

        [Required(ErrorMessage = "Icon is required.")]
        public IconIdentifier Icon { get; set; }

        [MinLength(1, ErrorMessage = "At least one text entry is required.")]
        public List<MenuLableTextViewModel> Texts { get; set; } = new();

        public List<LanguageListItemViewModel> AvailableLanguages { get; set; } = new();
    }
}
