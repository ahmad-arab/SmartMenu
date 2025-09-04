using SmartMenu.Data.Enums;

namespace SmartMenu.Models.Theme
{
    public class ThemeDesignerViewModel
    {
        public int MenuId { get; set; }

        public MenuThemeKey MenuThemeKey { get; set; } = MenuThemeKey.Default;
        public object MenuThemeModel { get; set; }

        public CategoryCardThemeKey CategoryCardThemeKey { get; set; } = CategoryCardThemeKey.Default;
        public object CategoryCardThemeModel { get; set; }

        public ItemCardThemeKey ItemCardThemeKey { get; set; } = ItemCardThemeKey.Default;
        public object ItemCardThemeModel { get; set; }

        public LableThemeKey LableThemeKey { get; set; } = LableThemeKey.Default;
        public object LableThemeModel { get; set; }

        public string PreviewUrl { get; set; } = string.Empty;
    }
}
