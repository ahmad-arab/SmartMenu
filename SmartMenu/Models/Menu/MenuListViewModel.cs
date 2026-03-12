namespace SmartMenu.Models.Menu
{
    public class MenuListViewModel
    {
        public int TenantAllowedMenusCount { get; set; }
        public int TenantActualMenusCount { get; set; }
        public bool IsAllowedToAddNewMenus { get; set; }
        public List<MenuListItemViewModel> Menus { get; set; }
    }
}
