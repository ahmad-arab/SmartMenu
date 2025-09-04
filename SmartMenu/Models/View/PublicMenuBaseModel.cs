using SmartMenu.Data.Enums;
using SmartMenu.Models.MenuCommand;
using SmartMenu.Models.MenuLable;

namespace SmartMenu.Models.View
{
    public class PublicMenuBaseModel
    {
        public int MenuId { get; set; }
        public string? Identifier { get; set; }
        public string MenuDefaultTitle { get; set; }
        public string MenuLogoUrl { get; set; }
        public string MenuCoverUrl { get; set; }
        public List<LanguageOption> AvailableLanguages { get; set; }
        public string SelectedLanguage { get; set; }
        public bool IsRtl { get; set; }
        public List<MenuLableListItemViewModel> MenuLables { get; set; }
        public List<MenuCommandListItemViewModel> MenuCommands { get; set; }

        public bool IsThemePreview { get; set; }

        public MenuThemeKey? MenuThemeKey { get; set; }
        public string? MenuThemeJson { get; set; }

        public CategoryCardThemeKey? CategoryCardThemeKey { get; set; }
        public string? CategoryCardThemeJson { get; set; }

        public ItemCardThemeKey? ItemCardThemeKey { get; set; }
        public string? ItemCardThemeJson { get; set; }

        public LableThemeKey? LableThemeKey { get; set; }
        public string? LableThemeJson { get; set; }
    }

    public class LanguageOption
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsRtl { get; set; }
        public bool IsDefault { get; set; }
    }
}
