using SmartMenu.Data.Enums;

namespace SmartMenu.Attributes.Theme
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class MenuThemeDefaultsAttribute : Attribute
    {
        public MenuThemeDefaultsAttribute(
            CategoryCardThemeKey categoryCardTheme,
            ItemCardThemeKey itemCardTheme,
            LableThemeKey lableTheme)
        {
            CategoryCardTheme = categoryCardTheme;
            ItemCardTheme = itemCardTheme;
            LableTheme = lableTheme;
        }

        public CategoryCardThemeKey CategoryCardTheme { get; }
        public ItemCardThemeKey ItemCardTheme { get; }
        public LableThemeKey LableTheme { get; }
    }
}
