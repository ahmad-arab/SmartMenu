using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class MenuLableText
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }

        public int MenuLableId { get; set; }
        public virtual MenuLable MenuLable { get; set; }

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }
    }
}
