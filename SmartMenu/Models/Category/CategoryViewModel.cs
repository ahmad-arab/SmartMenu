using SmartMenu.Models.Item;

namespace SmartMenu.Models.Category
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public string DefaultTitle { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public List<ItemListItemViewModel> Items { get; set; }
    }
}
