namespace RealEstateAPI.Features.Shared.Services;

/// <summary>
/// Interface for image processing and management services
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Processes uploaded image files and returns their URLs/paths
    /// </summary>
    /// <param name="imageFiles">Array of uploaded image files</param>
    /// <param name="subFolder">Subfolder within uploads directory (e.g., "properties", "users")</param>
    /// <returns>List of processed image URLs/paths</returns>
    Task<List<string>> ProcessImageFilesAsync(IFormFile[]? imageFiles, string subFolder);

    /// <summary>
    /// Validates if the uploaded file is a valid image
    /// </summary>
    /// <param name="file">File to validate</param>
    /// <returns>True if valid image file</returns>
    bool IsValidImageFile(IFormFile file);

    /// <summary>
    /// Deletes an image file from the server
    /// </summary>
    /// <param name="imageUrl">URL or path of the image to delete</param>
    /// <returns>True if successfully deleted</returns>
    Task<bool> DeleteImageAsync(string imageUrl);

    /// <summary>
    /// Deletes multiple image files from the server
    /// </summary>
    /// <param name="imageUrls">List of URLs or paths of images to delete</param>
    /// <returns>Number of successfully deleted images</returns>
    Task<int> DeleteImagesAsync(IEnumerable<string> imageUrls);

    /// <summary>
    /// Gets the allowed image file extensions
    /// </summary>
    /// <returns>Array of allowed extensions</returns>
    string[] GetAllowedExtensions();

    /// <summary>
    /// Gets the allowed image content types
    /// </summary>
    /// <returns>Array of allowed content types</returns>
    string[] GetAllowedContentTypes();

    /// <summary>
    /// Gets the maximum file size allowed for images
    /// </summary>
    /// <returns>Maximum file size in bytes</returns>
    long GetMaxFileSize();
}
