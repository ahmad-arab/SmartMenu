using SmartMenu.Models.Category;

namespace SmartMenu.Models.Menu
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public string DefaultTitle { get; set; }
        public string ImageUrl { get; set; }
        public List<CategoryListItemViewModel> Categories { get; set; }
    }
}
