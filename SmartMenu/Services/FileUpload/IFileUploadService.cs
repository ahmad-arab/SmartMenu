namespace SmartMenu.Services.FileUpload
{
    public interface IFileUploadService
    {
        /// <summary>
        /// Uploads an image file to the specified subfolder under wwwroot/uploads.
        /// Returns the relative URL (e.g. "/uploads/menu-images/guid.jpg"), or null if no file is provided.
        /// Throws <see cref="InvalidOperationException"/> when the file extension is not in the allowed list.
        /// </summary>
        Task<string?> UploadImageAsync(IFormFile? file, string subfolder);

        /// <summary>
        /// Returns true when the file extension belongs to the allowed image types.
        /// </summary>
        bool IsAllowedImageExtension(string fileName);

        /// <summary>
        /// Deletes the physical file that corresponds to a stored relative URL (e.g. "/uploads/...").
        /// Does nothing when <paramref name="imageUrl"/> is null or empty.
        /// </summary>
        void DeleteImage(string? imageUrl);
    }
}
