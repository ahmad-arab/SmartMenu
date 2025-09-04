using SmartMenu.Models.Language;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Models.Category
{
    public class CreateCategoryViewModel
    {
        public int MenuId { get; set; }

        [Display(Name = "Category Image")]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        public List<CategoryTitleAndDescriptionViewModel> TitlesAndDescriptions { get; set; } = new();
        public List<LanguageListItemViewModel> AvailableLanguages { get; set; } = new();
    }
    public class CategoryTitleAndDescriptionViewModel
    {
        public int LanguageId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Text { get; set; }

        public string? Description { get; set; }
    }
}
