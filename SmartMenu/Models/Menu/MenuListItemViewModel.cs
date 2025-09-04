namespace SmartMenu.Models.Menu
{
    public class MenuListItemViewModel
    {
        public int Id { get; set; }
        public string DefaultTitle { get; set; }
        public string ImageUrl { get; set; }
        public Dictionary<string, string> TitlesByLanguage { get; set; } = new();
    }
}
