using SmartMenu.Data.Enums;

namespace SmartMenu.Models.MenuLable
{
    public class MenuLableListItemViewModel
    {
        public int Id { get; set; }
        public IconIdentifier Icon { get; set; }
        public string DefaultText { get; set; }
        public string SummarizedDefaultText { get; set; }
        public string ThemeJson { get; set; }
        public Dictionary<string, string> TextsByLanguage { get; set; } = new();
    }
}
