using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class ItemTitle
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }

        public int ItemId { get; set; }
        public virtual Item Item { get; set; }

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }
    }
}
