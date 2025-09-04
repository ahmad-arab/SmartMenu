namespace SmartMenu.Models.Theme.Category
{
    public class CategoryCardTheme_DarkCircle
    {
        public int DiameterPx { get; set; } = 160;
        public string RingColor { get; set; } = "#ff7849";
        public int RingThicknessPx { get; set; } = 3;
        public string TitleColor { get; set; } = "#ffffff";
        public float TitleFontSize { get; set; } = 1.15f; // rem
        public string TitleFont { get; set; } = "";
        public float ShadowOpacity { get; set; } = 0.25f; // 0..1
    }
}