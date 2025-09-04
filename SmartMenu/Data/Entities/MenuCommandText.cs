using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class MenuCommandText
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }

        public int MenuCommandId { get; set; }
        public virtual MenuCommand MenuCommand { get; set; }

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }
    }
}
