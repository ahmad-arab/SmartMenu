using SmartMenu.Data.Enums;

namespace SmartMenu.Models.MenuCommand
{
    public class MenuCommandListItemViewModel
    {
        public int Id { get; set; }
        public IconIdentifier Icon { get; set; }
        public bool HasCustomerMessage { get; set; }
        public string DefaultText { get; set; }
        public string SystemMessage { get; set; }
        public Dictionary<string, string> TextsByLanguage { get; set; } = new();
    }
}
