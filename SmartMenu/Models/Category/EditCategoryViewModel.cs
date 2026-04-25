using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.Category
{
    public class EditCategoryViewModel
    {
        public int CategoryId { get; set; }

        [Display(Name = "Current Image")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Order is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Order must be between 0 and 2,147,483,647.")]
        public int Order { get; set; } = int.MaxValue;

        [Display(Name = "Category Image")]
        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }

        public List<CategoryTitleAndDescriptionViewModel> TitlesAndDescriptions { get; set; } = new();
        public List<LanguageListItemViewModel> AvailableLanguages { get; set; } = new();
    }
}
