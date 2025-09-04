namespace SmartMenu.Services.Whatsapp
{
    public interface IWhatsappService
    {
        Task SendMessageAsync(string to, string message);
        Task SendTemplateMessageAsync(string to, string menuTitle);
    }
}
