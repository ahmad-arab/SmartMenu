using QRCoder;

namespace SmartMenu.Services.Qr
{
    public class QrCodeService : IQrCodeService
    {
        public byte[] GeneratePng(string content, int pixelsPerModule = 10, string? darkColorHex = null, string? lightColorHex = null, bool drawQuietZones = true)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("QR content is required", nameof(content));

            darkColorHex ??= "#000000";
            lightColorHex ??= "#ffffff";

            using var generator = new QRCodeGenerator();
            using QRCodeData data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            var pngQr = new PngByteQRCode(data);
            var dark = System.Drawing.ColorTranslator.FromHtml(darkColorHex);
            var light = System.Drawing.ColorTranslator.FromHtml(lightColorHex);

            // Render PNG bytes directly
            return pngQr.GetGraphic(pixelsPerModule, dark, light, drawQuietZones);
        }
    }
}
