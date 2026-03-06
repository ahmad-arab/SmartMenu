using SmartMenu.Models.MenuCommand;
using SmartMenu.Models.Theme;
using SmartMenu.Models.View;

namespace SmartMenu.Services.PublicMenu
{
    public interface IPublicMenuService
    {
        Task<PublicMenuViewModel?> GetPublicMenuViewModelAsync(int menuId, string? lang, string? identifier, bool previewTheme, PreviewViewModel? previewModel);
        Task<PublicCategoryViewModel?> GetPublicCategoryViewModelAsync(int categoryId, string? lang, string? identifier, bool previewTheme, PreviewViewModel? previewModel);
        Task<(bool Success, string Message)> SendCommandAsync(SendMenuCommandRequest request);
    }
}
