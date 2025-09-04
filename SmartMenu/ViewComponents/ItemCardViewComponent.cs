using Microsoft.AspNetCore.Mvc;
using SmartMenu.Data.Enums;
using SmartMenu.Models.Category;
using SmartMenu.Models.View;

namespace SmartMenu.ViewComponents
{
    public class ItemCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ItemCardThemeKey? themKey, PublicCategoryItemViewModel model, string themeJson)
        {
            model.ThemeJson = themeJson;
            switch (themKey)
            {
                case ItemCardThemeKey.DarkCircle:
                    return View("DarkCircle", model);
                case ItemCardThemeKey.Default:
                default:
                    return View("Default", model);
            }
        }
    }
}
