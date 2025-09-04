namespace SmartMenu.Models.Theme.Item
{
    public class ItemCardTheme_Default
    {
        public int BorderRadius { get; set; } = 1; // int rem
        public int BorderWidth { get; set; } = 0; // int px
        public string BorderColor { get; set; } = "#000000";
        public string BackgroundColor { get; set; } = "#ffffff";

        public string TitleColor { get; set; } = "#000000";
        public float TitleFontSize { get; set; } = 1.25f; // in rem
        public string TitleFont { get; set; } = "";

        public string DescribtionColor { get; set; } = "#6b7280";
        public float DescribtionFontSize { get; set; } = 0.95f; // in rem
        public string DescribtionFont { get; set; } = "";

        public string PriceBadgeBackgroundColor { get; set; } = "#111827";
        public string PriceBadgeFontColor { get; set; } = "#ffffff";
        public float PriceBadgeFontSize { get; set; } = 0.9f; // in rem
        public string PriceBadgeFont { get; set; } = "";
        public int PriceBadgeFontWeight { get; set; } = 600;
    }
}
