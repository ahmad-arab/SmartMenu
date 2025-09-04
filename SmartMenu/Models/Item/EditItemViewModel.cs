using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.Item
{
    public class EditItemViewModel
    {
        public int ItemId { get; set; }

        [Display(Name = "Current Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Item Image")]
        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 999999.99, ErrorMessage = "Price must be between 0 and 999,999.99.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Price cannot have more than 2 decimal places.")]
        public decimal Price { get; set; } = 0.0m;

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; } = true;

        public List<ItemTitleAndDescriptionViewModel> TitlesAndDescriptions { get; set; } = new();
        public List<LanguageListItemViewModel> AvailableLanguages { get; set; } = new();
    }
}
