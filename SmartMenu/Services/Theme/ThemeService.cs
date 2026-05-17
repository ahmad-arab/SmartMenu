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
                case MenuThemeKey.OrientalSweets:
                    return DeserializeOrDefault(json, new MenuTheme_OrientalSweets(), getDefaultValues);
                case MenuThemeKey.DarkCircle:
                    return DeserializeOrDefault(json, new MenuTheme_DarkCircle(), getDefaultValues);
                case MenuThemeKey.BbqGrill:
                    return DeserializeOrDefault(json, new MenuTheme_BbqGrill(), getDefaultValues);
                case MenuThemeKey.DonerCanvas:
                    return DeserializeOrDefault(json, new MenuTheme_DonerCanvas(), getDefaultValues);
                case MenuThemeKey.RomeTricolore:
                    return DeserializeOrDefault(json, new MenuTheme_RomeTricolore(), getDefaultValues);
                case MenuThemeKey.Organic:
                    return DeserializeOrDefault(json, new MenuTheme_Organic(), getDefaultValues);
                case MenuThemeKey.Default:
                default:
                    return DeserializeOrDefault(json, new MenuTheme_Default(), getDefaultValues);
            }
        }

        public object GetCategoryCardThemeInstance(CategoryCardThemeKey key, string? json, bool getDefaultValues = false)
        {
            switch (key)
            {
                case CategoryCardThemeKey.OrientalSweets:
                    return DeserializeOrDefault(json, new CategoryCardTheme_OrientalSweets(), getDefaultValues);
                case CategoryCardThemeKey.DarkCircle:
                    return DeserializeOrDefault(json, new CategoryCardTheme_DarkCircle(), getDefaultValues);
                case CategoryCardThemeKey.BbqGrill:
                    return DeserializeOrDefault(json, new CategoryCardTheme_BbqGrill(), getDefaultValues);
                case CategoryCardThemeKey.DonerCanvas:
                    return DeserializeOrDefault(json, new CategoryCardTheme_DonerCanvas(), getDefaultValues);
                case CategoryCardThemeKey.RomeTricolore:
                    return DeserializeOrDefault(json, new CategoryCardTheme_RomeTricolore(), getDefaultValues);
                case CategoryCardThemeKey.Organic:
                    return DeserializeOrDefault(json, new CategoryCardTheme_Organic(), getDefaultValues);
                case CategoryCardThemeKey.Default:
                default:
                    return DeserializeOrDefault(json, new CategoryCardTheme_Default(), getDefaultValues);
            }
        }

        public object GetItemCardThemeInstance(ItemCardThemeKey key, string? json, bool getDefaultValues = false)
        {
            switch (key)
            {
                case ItemCardThemeKey.OrientalSweets:
                    return DeserializeOrDefault(json, new ItemCardTheme_OrientalSweets(), getDefaultValues);
                case ItemCardThemeKey.DarkCircle:
                    return DeserializeOrDefault(json, new ItemCardTheme_DarkCircle(), getDefaultValues);
                case ItemCardThemeKey.BbqGrill:
                    return DeserializeOrDefault(json, new ItemCardTheme_BbqGrill(), getDefaultValues);
                case ItemCardThemeKey.DonerCanvas:
                    return DeserializeOrDefault(json, new ItemCardTheme_DonerCanvas(), getDefaultValues);
                case ItemCardThemeKey.RomeTricolore:
                    return DeserializeOrDefault(json, new ItemCardTheme_RomeTricolore(), getDefaultValues);
                case ItemCardThemeKey.Organic:
                    return DeserializeOrDefault(json, new ItemCardTheme_Organic(), getDefaultValues);
                case ItemCardThemeKey.Default:
                default:
                    return DeserializeOrDefault(json, new ItemCardTheme_Default(), getDefaultValues);
            }
        }

        public object GetLableThemeInstance(LableThemeKey key, string? json, bool getDefaultValues = false)
        {
            switch (key)
            {
                case LableThemeKey.OrientalSweets:
                    return DeserializeOrDefault(json, new LableTheme_OrientalSweets(), getDefaultValues);
                case LableThemeKey.DarkCircle:
                    return DeserializeOrDefault(json, new LableTheme_DarkCircle(), getDefaultValues);
                case LableThemeKey.BbqGrill:
                    return DeserializeOrDefault(json, new LableTheme_BbqGrill(), getDefaultValues);
                case LableThemeKey.DonerCanvas:
                    return DeserializeOrDefault(json, new LableTheme_DonerCanvas(), getDefaultValues);
                case LableThemeKey.RomeTricolore:
                    return DeserializeOrDefault(json, new LableTheme_RomeTricolore(), getDefaultValues);
                case LableThemeKey.Organic:
                    return DeserializeOrDefault(json, new LableTheme_Organic(), getDefaultValues);
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
