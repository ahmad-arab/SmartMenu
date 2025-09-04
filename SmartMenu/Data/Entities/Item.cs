using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; } = 0.0m;
        public bool IsAvailable { get; set; } = true;

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public ICollection<ItemTitle> ItemTitles { get; set; } = new List<ItemTitle>();
        public ICollection<ItemDescription> ItemDescriptions { get; set; } = new List<ItemDescription>();
    }
}
