using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class MenuLanguage
    {
        [Key]
        public int Id { get; set; }

        public int MenuId { get; set; }
        public virtual Menu Menu { get; set; }

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }
    }
}
