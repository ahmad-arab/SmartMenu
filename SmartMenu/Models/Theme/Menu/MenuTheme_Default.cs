using SmartMenu.Data.Enums;

namespace SmartMenu.Models.Theme.Menu
{
    public class MenuTheme_Default
    {
        public string BodyBackgroundColor { get; set; } = "#ffffff";
        public int HeaderHeight { get; set; } = 180;
        public string ContentBackgroundColor { get; set; } = "#ffffff";
        public string TitleColor { get; set; } = "#000000";
        public float TitleFontSize { get; set; } = 2f; // in rem
        public string TitleFont { get; set; } = "";
        public string SearchboxBackgroundColor { get; set; } = "#ffffff";
        public string InputPalceHolderColor { get; set; } = "#808080";
    }
}
