namespace SmartMenu.Models.View
{
    public class PublicCategoryViewModel : PublicMenuBaseModel
    {
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string CategoryImageUrl { get; set; }
        public string ThemeJson { get; set; }
        public List<PublicCategoryItemViewModel> Items { get; set; }
    }

    public class PublicCategoryItemViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string DefaultTitle { get; set; }
        public Dictionary<string, string> TitlesByLanguage { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ThemeJson { get; set; }
    }
}
