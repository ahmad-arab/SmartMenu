using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.Item
{
    public class CreateItemViewModel
    {
        public int CategoryId { get; set; }

        [Display(Name = "Item Image")]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }
        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 999999.99, ErrorMessage = "Price must be between 0 and 999,999.99.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Price cannot have more than 2 decimal places.")]
        public decimal Price { get; set; } = 0.0m;

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; } = true;

        public List<ItemTitleAndDescriptionViewModel> TitlesAndDescriptions { get; set; } = new();
        public List<LanguageListItemViewModel> AvailableLanguages { get; set; } = new();
    }

    public class ItemTitleAndDescriptionViewModel
    {
        public int LanguageId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Text { get; set; }

        public string? Description { get; set; }
    }
}
