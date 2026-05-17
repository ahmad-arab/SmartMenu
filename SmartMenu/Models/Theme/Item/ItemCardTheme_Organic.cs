namespace SmartMenu.Models.Theme.Item
{
    public class ItemCardTheme_Organic
    {
        public string CardBackground { get; set; } = "#ffffff";
        public string BorderColor { get; set; } = "#e3edb6";
        public int BorderWidth { get; set; } = 1;
        public int BorderRadius { get; set; } = 22;

        public string TitleColor { get; set; } = "#000000";
        public float TitleFontSize { get; set; } = 1.4f;
        public string TitleFont { get; set; } = "";

        public string DescribtionColor { get; set; } = "#000000";
        public float DescribtionFontSize { get; set; } = 0.92f;
        public string DescribtionFont { get; set; } = "";

        public string PriceBadgeBackgroundColor { get; set; } = "#76a541";
        public string PriceBadgeFontColor { get; set; } = "#ffffff";
        public float PriceBadgeFontSize { get; set; } = 0.86f;
        public string PriceBadgeFont { get; set; } = "";
        public int PriceBadgeFontWeight { get; set; } = 800;

        public int ImageHeightPx { get; set; } = 182;
        public string ImageOverlayColor { get; set; } = "#e3edb6";
        public float ImageOverlayOpacity { get; set; } = 0.24f;
        public bool ShowDescription { get; set; } = true;
        public string AccentLeafColor { get; set; } = "#76a541";
        public string AccentFruitColor { get; set; } = "#e85124";
        public string AccentCreamColor { get; set; } = "#f3f3f4";
    }
}
