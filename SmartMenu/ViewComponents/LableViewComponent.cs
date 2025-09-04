using Microsoft.AspNetCore.Mvc;
using SmartMenu.Data.Enums;
using SmartMenu.Models.MenuLable;
using SmartMenu.Models.View;

namespace SmartMenu.ViewComponents
{
    public class LableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LableThemeKey? themKey, MenuLableListItemViewModel model, string themeJson)
        {
            model.ThemeJson = themeJson;
            switch (themKey)
            {
                case LableThemeKey.DarkCircle:
                    return View("DarkCircle", model);
                case LableThemeKey.Default:
                default:
                    return View("Default", model);
            }
        }
    }
}
