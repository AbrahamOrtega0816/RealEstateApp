namespace RealEstateAPI.Features.Shared.Services;

/// <summary>
/// Service for image processing and management
/// </summary>
public class ImageService : IImageService
{
    private readonly ILogger<ImageService> _logger;
    private readonly IWebHostEnvironment _environment;
    
    // Configuration constants
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
    private const string UploadsDirectory = "uploads";

    public ImageService(ILogger<ImageService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async Task<List<string>> ProcessImageFilesAsync(IFormFile[]? imageFiles, string subFolder)
    {
        var imageUrls = new List<string>();
        
        if (imageFiles == null || imageFiles.Length == 0)
        {
            return imageUrls;
        }

        try
        {
            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath ?? Directory.GetCurrentDirectory(), 
                                         "wwwroot", UploadsDirectory, subFolder);
            Directory.CreateDirectory(uploadsPath);

            foreach (var file in imageFiles)
            {
                if (file.Length > 0 && IsValidImageFile(file))
                {
                    // Generate unique filename
                    var fileName = GenerateUniqueFileName(file.FileName);
                    var filePath = Path.Combine(uploadsPath, fileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Add relative URL to list
                    imageUrls.Add($"/{UploadsDirectory}/{subFolder}/{fileName}");
                    
                    _logger.LogInformation("Image saved successfully: {FileName} in {SubFolder}", fileName, subFolder);
                }
                else
                {
                    _logger.LogWarning("Invalid image file rejected: {FileName}. Size: {Size} bytes, ContentType: {ContentType}", 
                        file.FileName, file.Length, file.ContentType);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image files for subfolder: {SubFolder}", subFolder);
            throw;
        }

        return imageUrls;
    }

    public bool IsValidImageFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        if (file.Length > MaxFileSize)
        {
            _logger.LogWarning("File size too large: {FileName} ({Size} bytes). Max allowed: {MaxSize} bytes", 
                file.FileName, file.Length, MaxFileSize);
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var contentType = file.ContentType.ToLowerInvariant();
        
        var isValidExtension = AllowedExtensions.Contains(extension);
        var isValidContentType = AllowedContentTypes.Contains(contentType);
        
        if (!isValidExtension)
        {
            _logger.LogWarning("Invalid file extension: {Extension} for file: {FileName}", extension, file.FileName);
        }
        
        if (!isValidContentType)
        {
            _logger.LogWarning("Invalid content type: {ContentType} for file: {FileName}", contentType, file.FileName);
        }
        
        return isValidExtension && isValidContentType;
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return false;

            // Convert URL to file path
            var filePath = GetPhysicalPath(imageUrl);
            
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
                _logger.LogInformation("Image deleted successfully: {ImageUrl}", imageUrl);
                return true;
            }
            
            _logger.LogWarning("Image file not found for deletion: {ImageUrl}", imageUrl);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image: {ImageUrl}", imageUrl);
            return false;
        }
    }

    public async Task<int> DeleteImagesAsync(IEnumerable<string> imageUrls)
    {
        if (imageUrls == null)
            return 0;

        var deletedCount = 0;
        var deleteUrls = imageUrls.ToList();

        foreach (var imageUrl in deleteUrls)
        {
            if (await DeleteImageAsync(imageUrl))
            {
                deletedCount++;
            }
        }

        _logger.LogInformation("Deleted {DeletedCount} out of {TotalCount} images", deletedCount, deleteUrls.Count);
        return deletedCount;
    }

    public string[] GetAllowedExtensions()
    {
        return AllowedExtensions;
    }

    public string[] GetAllowedContentTypes()
    {
        return AllowedContentTypes;
    }

    public long GetMaxFileSize()
    {
        return MaxFileSize;
    }

    /// <summary>
    /// Generates a unique filename to prevent conflicts
    /// </summary>
    /// <param name="originalFileName">Original file name</param>
    /// <returns>Unique file name</returns>
    private static string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        
        // Sanitize filename - remove invalid characters
        var sanitizedName = string.Join("_", nameWithoutExtension.Split(Path.GetInvalidFileNameChars()));
        
        // Generate unique filename with timestamp and GUID
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var uniqueId = Guid.NewGuid().ToString("N")[..8]; // First 8 characters of GUID
        
        return $"{timestamp}_{uniqueId}_{sanitizedName}{extension}";
    }

    /// <summary>
    /// Converts a relative URL to physical file path
    /// </summary>
    /// <param name="imageUrl">Relative image URL</param>
    /// <returns>Physical file path</returns>
    private string GetPhysicalPath(string imageUrl)
    {
        // Remove leading slash if present
        var relativePath = imageUrl.TrimStart('/');
        
        var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        return Path.Combine(webRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
    }
}
