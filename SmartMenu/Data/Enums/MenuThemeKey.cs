using SmartMenu.Attributes.Theme;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Enums
{
    public enum MenuThemeKey
    {
        [Display(Name = "Default")]
        [MenuThemeDefaults(CategoryCardThemeKey.Default, ItemCardThemeKey.Default, LableThemeKey.Default)]
        Default,

        [Display(Name = "Dark Circle")]
        [OnePageMenu]
        [MenuThemeDefaults(CategoryCardThemeKey.DarkCircle, ItemCardThemeKey.DarkCircle, LableThemeKey.DarkCircle)]
        DarkCircle,
    }

    public enum CategoryCardThemeKey
    {
        [Display(Name = "Default")]
        Default,

        [Display(Name = "Dark Circle")]
        DarkCircle,
    }

    public enum ItemCardThemeKey
    {
        [Display(Name = "Default")]
        Default,

        [Display(Name = "Dark Circle")]
        DarkCircle,
    }

    public enum LableThemeKey
    {
        [Display(Name = "Default")]
        Default,

        [Display(Name = "Dark Circle")]
        DarkCircle,
    }
}
