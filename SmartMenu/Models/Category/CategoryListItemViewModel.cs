namespace SmartMenu.Models.Category
{
    public class CategoryListItemViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string DefaultTitle { get; set; }
        public string ThemeJson { get; set; }
        public string? LinkUrl { get; set; }
        public Dictionary<string, string> TitlesByLanguage { get; set; } = new(); 
    }
}
