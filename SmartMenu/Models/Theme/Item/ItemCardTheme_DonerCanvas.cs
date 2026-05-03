namespace SmartMenu.Models.Theme.Item
{
    public class ItemCardTheme_DonerCanvas
    {
        public string CardBackground { get; set; } = "#0d0d0d";
        public string BorderColor { get; set; } = "#f95700";
        public int BorderWidth { get; set; } = 1;
        public int BorderRadius { get; set; } = 22;

        public string TitleColor { get; set; } = "#fff6ee";
        public float TitleFontSize { get; set; } = 1.18f;
        public string TitleFont { get; set; } = "";

        public string DescribtionColor { get; set; } = "#d8d1ca";
        public float DescribtionFontSize { get; set; } = 0.92f;
        public string DescribtionFont { get; set; } = "";

        public string PriceBadgeBackgroundColor { get; set; } = "#f95700";
        public string PriceBadgeFontColor { get; set; } = "#140b05";
        public float PriceBadgeFontSize { get; set; } = 0.95f;
        public string PriceBadgeFont { get; set; } = "";
        public int PriceBadgeFontWeight { get; set; } = 800;

        public int ImageHeightPx { get; set; } = 300;
        public string ImageOverlayColor { get; set; } = "#000000";
        public float ImageOverlayOpacity { get; set; } = 0.22f;
        public float GrillMarkOpacity { get; set; } = 0.1f;
        public bool ShowDescription { get; set; } = true;
        public string EmberGlowColor { get; set; } = "#ff9a3d";
    }
}