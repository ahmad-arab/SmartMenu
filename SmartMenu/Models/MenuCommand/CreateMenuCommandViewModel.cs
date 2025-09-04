using SmartMenu.Data.Enums;
using SmartMenu.Models.Category;
using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.MenuCommand
{
    public class CreateMenuCommandViewModel
    {
        [Required(ErrorMessage = "Menu is required.")]
        public int MenuId { get; set; }

        [Required(ErrorMessage = "Icon is required.")]
        public IconIdentifier Icon { get; set; }

        [Required(ErrorMessage = "Has Customer Message is required.")]
        public bool HasCustomerMessage { get; set; } = false;

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(1000, ErrorMessage = "Message cannot be longer than 1000 characters.")]
        public string SystemMessage { get; set; }

        [MinLength(1, ErrorMessage = "At least one text entry is required.")]
        public List<MenuCommandTextViewModel> Texts { get; set; } = new();

        // Staff selection
        public List<int> SelectedStaffIds { get; set; } = new();
        public List<MenuStaffOptionViewModel> AvailableStaff { get; set; } = new();

        public List<LanguageListItemViewModel> AvailableLanguages { get; set; } = new();
    }

    public class MenuCommandTextViewModel
    {
        public int LanguageId { get; set; }

        [Required(ErrorMessage = "Text is required.")]
        [StringLength(1000, ErrorMessage = "Text cannot be longer than 1000 characters.")]
        public string Text { get; set; }
    }

    public class MenuStaffOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}
