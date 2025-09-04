using SmartMenu.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class MenuCommand
    {
        [Key]
        public int Id { get; set; }
        public IconIdentifier Icon { get; set; }
        public bool HasCustomerMessage { get; set; }
        public string SystemMessage { get; set; }

        public int MenuId { get; set; }
        public virtual Menu Menu { get; set; }

        public ICollection<MenuCommandText> MenuCommandTexts { get; set; } = new List<MenuCommandText>();

        // Mapping to selected staffs
        public ICollection<MenuCommandStaff> MenuCommandStaffs { get; set; } = new List<MenuCommandStaff>();
    }
}
