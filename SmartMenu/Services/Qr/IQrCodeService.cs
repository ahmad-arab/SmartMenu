namespace SmartMenu.Services.Qr
{
    public interface IQrCodeService
    {
        byte[] GeneratePng(string content, int pixelsPerModule = 10, string? darkColorHex = null, string? lightColorHex = null, bool drawQuietZones = true);
    }
}
