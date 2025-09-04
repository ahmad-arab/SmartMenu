using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Data.Entities
{
    public class Language
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRtl { get; set; }
        public bool IsDefault { get; set; } = false;

        public int TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }

        public ICollection<LanguageText> LanguageTexts { get; set; } = new List<LanguageText>();

        public ICollection<CategoryTitle> CategoryTitles { get; set; } = new List<CategoryTitle>();
        public ICollection<CategoryDescription> CategoryDescriptions { get; set; } = new List<CategoryDescription>();

        public ICollection<ItemTitle> ItemTitles { get; set; } = new List<ItemTitle>();
        public ICollection<ItemDescription> ItemDescriptions { get; set; } = new List<ItemDescription>();

        public ICollection<MenuLanguage> MenuLanguages { get; set; } = new List<MenuLanguage>();
        public ICollection<MenuLableText> MenuLableTexts { get; set; } = new List<MenuLableText>();
        public ICollection<MenuCommandText> MenuCommandTexts { get; set; } = new List<MenuCommandText>();
        public ICollection<MenuTitle> MenuTitles { get; set; } = new List<MenuTitle>();
    }
}
