namespace SmartMenu.Models.Theme.Lable
{
    public class LableTheme_Default
    {
        public int BorderRadius { get; set; } = 50; // int rem
        public int BorderWidth { get; set; } = 1; // int px
        public string BorderColor { get; set; } = "#e5e7eb";
        public string BackgroundColorStart { get; set; } = "#ffffff";
        public string BackgroundColorEnd { get; set; } = "#f2f4f7";
        public int BackgroundColorAngle { get; set; } = 180;

        public string TextColor { get; set; } = "#000000";
        public float TextFontSize { get; set; } = 1f; // in rem
        public string TextFont { get; set; } = "";
    }
}
