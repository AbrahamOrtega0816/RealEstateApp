namespace RealEstateAPI.Features.Shared.Services;

/// <summary>
/// Service for URL generation and manipulation
/// </summary>
public class UrlService : IUrlService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UrlService> _logger;

    public UrlService(IHttpContextAccessor httpContextAccessor, ILogger<UrlService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Converts a relative path to a full URL using the current request context
    /// </summary>
    /// <param name="relativePath">The relative path to convert</param>
    /// <returns>The full URL</returns>
    public string GetFullUrl(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            _logger.LogDebug("GetFullUrl: relativePath is null or empty");
            return string.Empty;
        }

        // If it's already a full URL, return as is
        if (relativePath.StartsWith("http://") || relativePath.StartsWith("https://"))
        {
            _logger.LogDebug("GetFullUrl: relativePath is already a full URL: {RelativePath}", relativePath);
            return relativePath;
        }

        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request != null)
            {
                var request = httpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                var fullUrl = $"{baseUrl}{relativePath}";
                _logger.LogDebug("GetFullUrl: Generated full URL: {FullUrl} from base: {BaseUrl} and path: {RelativePath}", 
                    fullUrl, baseUrl, relativePath);
                return fullUrl;
            }
            else
            {
                _logger.LogWarning("GetFullUrl: HttpContext or Request is null");
                // Fallback to localhost for development
                var fallbackUrl = $"http://localhost:5000{relativePath}";
                _logger.LogDebug("GetFullUrl: Using fallback URL: {FallbackUrl}", fallbackUrl);
                return fallbackUrl;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error generating full URL for path: {RelativePath}", relativePath);
            // Return relative path as fallback
            return relativePath;
        }
    }

    /// <summary>
    /// Converts a collection of relative paths to full URLs
    /// </summary>
    /// <param name="relativePaths">The relative paths to convert</param>
    /// <returns>The full URLs</returns>
    public List<string> GetFullUrls(IEnumerable<string> relativePaths)
    {
        if (relativePaths == null)
        {
            return new List<string>();
        }

        return relativePaths.Select(GetFullUrl).ToList();
    }
}
