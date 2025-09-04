using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.Category
{
    public class EditCategoryViewModel
    {
        public int CategoryId { get; set; }

        [Display(Name = "Current Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Category Image")]
        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }

        public List<CategoryTitleAndDescriptionViewModel> TitlesAndDescriptions { get; set; } = new();
        public List<LanguageListItemViewModel> AvailableLanguages { get; set; } = new();
    }
}
