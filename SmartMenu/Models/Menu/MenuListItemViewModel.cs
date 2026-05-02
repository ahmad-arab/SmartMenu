namespace SmartMenu.Models.Menu
{
    public class MenuListItemViewModel
    {
        public int Id { get; set; }
        public string DefaultTitle { get; set; }
        public string ImageUrl { get; set; }
        public string? HeroSubtitleText { get; set; }
        public string? CategoryIndexTitleText { get; set; }
        public Dictionary<string, string> TitlesByLanguage { get; set; } = new();
    }
}
