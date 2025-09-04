namespace SmartMenu.Models.Item
{
    public class ItemListItemViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string DefaultTitle { get; set; }
        public Dictionary<string, string> TitlesByLanguage { get; set; } = new();
    }
}
