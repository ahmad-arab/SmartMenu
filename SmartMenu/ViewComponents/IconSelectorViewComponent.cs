using Microsoft.AspNetCore.Mvc;
using SmartMenu.Data.Enums;

namespace SmartMenu.ViewComponents
{
    public class IconSelectorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string name, IconIdentifier? selected, bool isRequired = false)
        {
            var icons = Enum.GetValues(typeof(IconIdentifier)).Cast<IconIdentifier>().ToList();
            ViewData["FieldName"] = name;
            ViewData["IsRequired"] = isRequired;
            return View((icons, selected));
        }
    }
}
