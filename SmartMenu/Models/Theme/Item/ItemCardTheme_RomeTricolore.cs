namespace SmartMenu.Models.Theme.Item
{
    public class ItemCardTheme_RomeTricolore
    {
        public string CardBackground { get; set; } = "#fffdfa";
        public string BorderColor { get; set; } = "#d6c3a1";
        public int BorderWidth { get; set; } = 1;
        public int BorderRadius { get; set; } = 20;

        public string TitleColor { get; set; } = "#fff6df";
        public float TitleFontSize { get; set; } = 2.0f;
        public string TitleFont { get; set; } = "";

        public string DescribtionColor { get; set; } = "#cac5bb";
        public float DescribtionFontSize { get; set; } = 0.88f;
        public string DescribtionFont { get; set; } = "";

        public string PriceBadgeBackgroundColor { get; set; } = "#8c1d1d";
        public string PriceBadgeFontColor { get; set; } = "#fff7ef";
        public float PriceBadgeFontSize { get; set; } = 0.82f;
        public string PriceBadgeFont { get; set; } = "";
        public int PriceBadgeFontWeight { get; set; } = 800;

        public int ImageHeightPx { get; set; } = 164;
        public string ImageOverlayColor { get; set; } = "#111827";
        public float ImageOverlayOpacity { get; set; } = 0.18f;
        public bool ShowDescription { get; set; } = true;
        public string AccentGreen { get; set; } = "#008c45";
        public string AccentRed { get; set; } = "#ce2b37";
    }
}
