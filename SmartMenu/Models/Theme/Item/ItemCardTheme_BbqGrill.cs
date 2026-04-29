namespace SmartMenu.Models.Theme.Item
{
    public class ItemCardTheme_BbqGrill
    {
        public string CardBackground { get; set; } = "#1e1008";
        public string BorderColor { get; set; } = "#772f5c";
        public int BorderWidth { get; set; } = 1;
        public int BorderRadius { get; set; } = 12;

        public string TitleColor { get; set; } = "#f5dfc0";
        public float TitleFontSize { get; set; } = 1.15f;
        public string TitleFont { get; set; } = "";

        public string DescribtionColor { get; set; } = "#c89a74";
        public float DescribtionFontSize { get; set; } = 0.9f;
        public string DescribtionFont { get; set; } = "";

        public string PriceBadgeBackgroundColor { get; set; } = "#772f5c";
        public string PriceBadgeFontColor { get; set; } = "#ffd5a8";
        public float PriceBadgeFontSize { get; set; } = 0.9f;
        public string PriceBadgeFont { get; set; } = "";
        public int PriceBadgeFontWeight { get; set; } = 700;

        public int ImageHeightPx { get; set; } = 280;
        public string ImageOverlayColor { get; set; } = "#000000";
        public float ImageOverlayOpacity { get; set; } = 0.1f;
        /// <summary>Opacity of the repeating diagonal grill-mark lines on the card (0–1).</summary>
        public float GrillMarkOpacity { get; set; } = 0.07f;
        /// <summary>Show the item description text on the card.</summary>
        public bool ShowDescription { get; set; } = true;
        /// <summary>Color of the ember glow line at the bottom edge of the card.</summary>
        public string EmberGlowColor { get; set; } = "#e8612a";
    }
}
