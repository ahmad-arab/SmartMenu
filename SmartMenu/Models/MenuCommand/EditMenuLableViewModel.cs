using SmartMenu.Data.Enums;
using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.MenuCommand
{
    public class EditMenuCommandViewModel
    {
        [Required(ErrorMessage = "Menu command ID is required.")]
        public int MenuCommandId { get; set; }

        [Required(ErrorMessage = "Icon is required.")]
        public IconIdentifier Icon { get; set; }

        [Required(ErrorMessage = "Has Customer Message is required.")] 
        public bool HasCustomerMessage { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(1000, ErrorMessage = "Message cannot be longer than 1000 characters.")]
        public string SystemMessage { get; set; }

        [MinLength(1, ErrorMessage = "At least one text entry is required.")]
        public List<MenuCommandTextViewModel> Texts{ get; set; } = new();

        public List<int> SelectedStaffIds { get; set; } = new();
        public List<MenuStaffOptionViewModel> AvailableStaff { get; set; } = new();

        public List<LanguageListItemViewModel> AvailableLanguages { get; set; } = new();
    }
}
