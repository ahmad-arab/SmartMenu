using SmartMenu.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class MenuLable
    {
        [Key]
        public int Id { get; set; }
        public IconIdentifier Icon { get; set; }

        public int MenuId { get; set; }
        public virtual Menu Menu { get; set; }

        public ICollection<MenuLableText> MenuLableTexts { get; set; } = new List<MenuLableText>();
    }
}
