using System.Text.Json;
using SmartMenu.Data.Enums;
using SmartMenu.Models.Theme.Menu;
using SmartMenu.Models.Theme.Category;
using SmartMenu.Models.Theme.Item;
using SmartMenu.Models.Theme.Lable;
using SmartMenu.Attributes.Theme;

namespace SmartMenu.Services.Theme
{
    public class ThemeService : IThemeService
    {
        public ThemeService() { }

        public object GetMenuThemeInstance(MenuThemeKey key, string? json, bool getDefaultValues = false)
        {
            switch (key)
            {
                case MenuThemeKey.DarkCircle:
                    return DeserializeOrDefault(json, new MenuTheme_DarkCircle(), getDefaultValues);
                case MenuThemeKey.Default:
                default:
                    return DeserializeOrDefault(json, new MenuTheme_Default(), getDefaultValues);
            }
        }

        public object GetCategoryCardThemeInstance(CategoryCardThemeKey key, string? json, bool getDefaultValues = false)
        {
            switch (key)
            {
                case CategoryCardThemeKey.DarkCircle:
                    return DeserializeOrDefault(json, new CategoryCardTheme_DarkCircle(), getDefaultValues);
                case CategoryCardThemeKey.Default:
                default:
                    return DeserializeOrDefault(json, new CategoryCardTheme_Default(), getDefaultValues);
            }
        }

        public object GetItemCardThemeInstance(ItemCardThemeKey key, string? json, bool getDefaultValues = false)
        {
            switch (key)
            {
                case ItemCardThemeKey.DarkCircle:
                    return DeserializeOrDefault(json, new ItemCardTheme_DarkCircle(), getDefaultValues);
                case ItemCardThemeKey.Default:
                default:
                    return DeserializeOrDefault(json, new ItemCardTheme_Default(), getDefaultValues);
            }
        }

        public object GetLableThemeInstance(LableThemeKey key, string? json, bool getDefaultValues = false)
        {
            switch (key)
            {
                case LableThemeKey.DarkCircle:
                    return DeserializeOrDefault(json, new LableTheme_DarkCircle(), getDefaultValues);
                case LableThemeKey.Default:
                default:
                    return DeserializeOrDefault(json, new LableTheme_Default(), getDefaultValues);
            }
        }

        public bool IsOnePageMenu(MenuThemeKey key)
        {
            var mem = typeof(MenuThemeKey).GetMember(key.ToString()).FirstOrDefault();
            if (mem == null) return false;
            return Attribute.IsDefined(mem, typeof(OnePageMenuAttribute));
        }

        private static T DeserializeOrDefault<T>(string? json, T @default, bool getDefaultValues = false)
        {
            if (string.IsNullOrWhiteSpace(json) || getDefaultValues) return @default;
            try
            {
                var obj = System.Text.Json.JsonSerializer.Deserialize<T>(json);
                return obj != null ? obj : @default;
            }
            catch
            {
                return @default;
            }
        }
    }
}
