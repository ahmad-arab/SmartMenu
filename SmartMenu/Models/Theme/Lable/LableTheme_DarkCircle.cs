namespace SmartMenu.Models.Theme.Lable
{
    public class LableTheme_DarkCircle
    {
        public int BorderRadius { get; set; } = 50; // rem-like pill
        public int BorderWidth { get; set; } = 1; // px
        public string BorderColor { get; set; } = "#334155";
        public string BackgroundColorStart { get; set; } = "#0f1620";
        public string BackgroundColorEnd { get; set; } = "#101826";
        public int BackgroundColorAngle { get; set; } = 180;

        public string TextColor { get; set; } = "#e5e7eb";
        public float TextFontSize { get; set; } = 0.95f; // rem
        public string TextFont { get; set; } = "";
        public string IconTint { get; set; } = "#ff7849";
    }
}