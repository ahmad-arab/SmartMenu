using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }

        public int MenuId { get; set; }
        public virtual Menu Menu { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<CategoryTitle> CategoryTitles { get; set; } = new List<CategoryTitle>();
        public ICollection<CategoryDescription> CategoryDescriptions { get; set; } = new List<CategoryDescription>();
    }
}
