using SmartMenu.Models.Category;

namespace SmartMenu.Models.View
{
    public class PublicMenuViewModel : PublicMenuBaseModel
    {
        public List<CategoryListItemViewModel> Categories { get; set; }
        public string ThemeJson { get; set; }

        // For one-page themes: items grouped by category
        public List<MenuCategoryWithItemsViewModel> CategoriesWithItems { get; set; } = new();
    }

    public class MenuCategoryWithItemsViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string? CategoryImageUrl { get; set; }
        public List<PublicCategoryItemViewModel> Items { get; set; } = new();
    }
}
