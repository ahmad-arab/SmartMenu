using SmartMenu.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class Menu
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }

        public int TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }

        public MenuThemeKey? MenuThemeKey { get; set; }
        public string? MenuThemeJson { get; set; }

        public CategoryCardThemeKey? CategoryCardThemeKey { get; set; }
        public string? CategoryCardThemeJson { get; set; }

        public ItemCardThemeKey? ItemCardThemeKey { get; set; }
        public string? ItemCardThemeJson { get; set; }

        public LableThemeKey? LableThemeKey { get; set; }
        public string? LableThemeJson { get; set; }

        public ICollection<MenuTitle> MenuTitles { get; set; } = new List<MenuTitle>();
        public ICollection<Category> Categorys { get; set; } = new List<Category>();
        public ICollection<MenuLable> MenuLables { get; set; } = new List<MenuLable>();
        public ICollection<MenuCommand> MenuCommands { get; set; } = new List<MenuCommand>();
        public ICollection<MenuLanguage> MenuLanguages { get; set; } = new List<MenuLanguage>();
        public ICollection<MenuStaff> MenuStaffs { get; set; } = new List<MenuStaff>();
    }
}
