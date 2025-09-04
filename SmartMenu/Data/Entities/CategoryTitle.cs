using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class CategoryTitle
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }
    }
}
