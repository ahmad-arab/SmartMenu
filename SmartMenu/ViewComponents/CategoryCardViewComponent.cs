using Microsoft.AspNetCore.Mvc;
using SmartMenu.Data.Enums;
using SmartMenu.Models.Category;

namespace SmartMenu.ViewComponents
{
    public class CategoryCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CategoryCardThemeKey? themKey, CategoryListItemViewModel model, string themeJson, string? linkUrl = null)
        {
            model.ThemeJson = themeJson;
            if (!string.IsNullOrWhiteSpace(linkUrl))
            {
                model.LinkUrl = linkUrl;
            }
            switch (themKey)
            {
                case CategoryCardThemeKey.DarkCircle:
                    return View("DarkCircle", model);
                case CategoryCardThemeKey.Default:
                default:
                    return View("Default", model);
            }
        }
    }
}
