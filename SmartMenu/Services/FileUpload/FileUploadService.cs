namespace SmartMenu.Services.FileUpload
{
    public class FileUploadService : IFileUploadService
    {
        private static readonly string[] AllowedImageExtensions =
            [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"];

        private readonly IWebHostEnvironment _env;

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <inheritdoc />
        public async Task<string?> UploadImageAsync(IFormFile? file, string subfolder)
        {
            if (file == null || file.Length == 0)
                return null;

            if (!IsSubfolderSafe(subfolder))
                throw new ArgumentException("Invalid subfolder name.", nameof(subfolder));

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(ext))
                throw new InvalidOperationException("Only image files are allowed.");

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", subfolder);
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{subfolder}/{fileName}";
        }

        /// <inheritdoc />
        public bool IsAllowedImageExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return AllowedImageExtensions.Contains(ext);
        }

        /// <inheritdoc />
        public void DeleteImage(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            var relativePath = imageUrl.StartsWith('/') ? imageUrl[1..] : imageUrl;
            var fullPath = Path.Combine(_env.WebRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        private static bool IsSubfolderSafe(string subfolder) =>
            !string.IsNullOrWhiteSpace(subfolder) &&
            subfolder.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 &&
            !subfolder.Contains("..") &&
            !subfolder.Contains('/') &&
            !subfolder.Contains('\\');
    }
}
