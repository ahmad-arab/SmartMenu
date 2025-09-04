using SmartMenu.Data.Enums;

namespace SmartMenu.Models.Theme
{
    public class PreviewViewModel
    {
        public MenuThemeKey? MenuThemeKey { get; set; }
        public string? MenuThemeJson { get; set; }

        public CategoryCardThemeKey? CategoryCardThemeKey { get; set; }
        public string? CategoryCardThemeJson { get; set; }

        public ItemCardThemeKey? ItemCardThemeKey { get; set; }
        public string? ItemCardThemeJson { get; set; }

        public LableThemeKey? LableThemeKey { get; set; }
        public string? LableThemeJson { get; set; }
    }
}
