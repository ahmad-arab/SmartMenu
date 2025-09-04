namespace SmartMenu.Models.Theme.Item
{
    public class ItemCardTheme_DarkCircle
    {
        public string CardBackground { get; set; } = "#0f1620";
        public int BorderRadius { get; set; } = 1; // rem
        public int BorderWidth { get; set; } = 0; // px
        public string BorderColor { get; set; } = "#000000";

        public int ThumbDiameterPx { get; set; } = 140;

        public string TitleColor { get; set; } = "#e5e7eb";
        public float TitleFontSize { get; set; } = 1.2f; // rem
        public string TitleFont { get; set; } = "";

        public string DescribtionColor { get; set; } = "#94a3b8";
        public float DescribtionFontSize { get; set; } = 0.95f; // rem
        public string DescribtionFont { get; set; } = "";

        public string PriceBadgeBackgroundColor { get; set; } = "#111827";
        public string PriceBadgeFontColor { get; set; } = "#ffffff";
        public float PriceBadgeFontSize { get; set; } = 0.9f; // rem
        public string PriceBadgeFont { get; set; } = "";
        public int PriceBadgeFontWeight { get; set; } = 600;
        public string AccentRingColor { get; set; } = "#ff7849";
        public int AccentRingThicknessPx { get; set; } = 3;
    }
}