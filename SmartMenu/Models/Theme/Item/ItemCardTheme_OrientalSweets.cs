namespace SmartMenu.Models.Theme.Item
{
    public class ItemCardTheme_OrientalSweets
    {
        public string CardBackground { get; set; } = "#fffdf7";
        public string BorderColor { get; set; } = "#e0bc7a";
        public int BorderWidth { get; set; } = 1;
        public int BorderRadius { get; set; } = 16;

        public string TitleColor { get; set; } = "#5e3616";
        public float TitleFontSize { get; set; } = 1.2f;
        public string TitleFont { get; set; } = "";

        public string DescribtionColor { get; set; } = "#7a5a38";
        public float DescribtionFontSize { get; set; } = 0.95f;
        public string DescribtionFont { get; set; } = "";

        public string PriceBadgeBackgroundColor { get; set; } = "#8c5522";
        public string PriceBadgeFontColor { get; set; } = "#fff3d2";
        public float PriceBadgeFontSize { get; set; } = 0.9f;
        public string PriceBadgeFont { get; set; } = "";
        public int PriceBadgeFontWeight { get; set; } = 700;

        public int ImageHeightPx { get; set; } = 180;
        public string ImageOverlayColor { get; set; } = "#000000";
        public float ImageOverlayOpacity { get; set; } = 0.1f;
    }
}
