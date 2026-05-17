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

        [Display(Name = "Oriental Sweets")]
        [OnePageMenu]
        [MenuThemeDefaults(CategoryCardThemeKey.OrientalSweets, ItemCardThemeKey.OrientalSweets, LableThemeKey.OrientalSweets)]
        OrientalSweets,

        [Display(Name = "BBQ Grill")]
        [OnePageMenu]
        [MenuThemeDefaults(CategoryCardThemeKey.BbqGrill, ItemCardThemeKey.BbqGrill, LableThemeKey.BbqGrill)]
        BbqGrill,

        [Display(Name = "Doner Canvas")]
        [OnePageMenu]
        [MenuThemeDefaults(CategoryCardThemeKey.DonerCanvas, ItemCardThemeKey.DonerCanvas, LableThemeKey.DonerCanvas)]
        DonerCanvas,

        [Display(Name = "Rome Tricolore")]
        [MenuThemeDefaults(CategoryCardThemeKey.RomeTricolore, ItemCardThemeKey.RomeTricolore, LableThemeKey.RomeTricolore)]
        RomeTricolore,

        [Display(Name = "Organic")]
        [MenuThemeDefaults(CategoryCardThemeKey.Organic, ItemCardThemeKey.Organic, LableThemeKey.Organic)]
        Organic,
    }

    public enum CategoryCardThemeKey
    {
        [Display(Name = "Default")]
        Default,

        [Display(Name = "Dark Circle")]
        DarkCircle,

        [Display(Name = "Oriental Sweets")]
        OrientalSweets,

        [Display(Name = "BBQ Grill")]
        BbqGrill,

        [Display(Name = "Doner Canvas")]
        DonerCanvas,

        [Display(Name = "Rome Tricolore")]
        RomeTricolore,

        [Display(Name = "Organic")]
        Organic,
    }

    public enum ItemCardThemeKey
    {
        [Display(Name = "Default")]
        Default,

        [Display(Name = "Dark Circle")]
        DarkCircle,

        [Display(Name = "Oriental Sweets")]
        OrientalSweets,

        [Display(Name = "BBQ Grill")]
        BbqGrill,

        [Display(Name = "Doner Canvas")]
        DonerCanvas,

        [Display(Name = "Rome Tricolore")]
        RomeTricolore,

        [Display(Name = "Organic")]
        Organic,
    }

    public enum LableThemeKey
    {
        [Display(Name = "Default")]
        Default,

        [Display(Name = "Dark Circle")]
        DarkCircle,

        [Display(Name = "Oriental Sweets")]
        OrientalSweets,

        [Display(Name = "BBQ Grill")]
        BbqGrill,

        [Display(Name = "Doner Canvas")]
        DonerCanvas,

        [Display(Name = "Rome Tricolore")]
        RomeTricolore,

        [Display(Name = "Organic")]
        Organic,
    }
}
