using SmartMenu.Data.Enums;

namespace SmartMenu.Services.Theme
{
    public interface IThemeService
    {
        // Factories that return a typed theme instance using provided json or defaults
        object GetMenuThemeInstance(MenuThemeKey key, string? json, bool getDefaultValues = false);
        object GetCategoryCardThemeInstance(CategoryCardThemeKey key, string? json, bool getDefaultValues = false);
        object GetItemCardThemeInstance(ItemCardThemeKey key, string? json, bool getDefaultValues = false);
        object GetLableThemeInstance(LableThemeKey key, string? json, bool getDefaultValues = false);

        // Theme metadata helpers
        bool IsOnePageMenu(MenuThemeKey key);
    }
}
