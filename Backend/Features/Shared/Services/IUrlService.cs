namespace RealEstateAPI.Features.Shared.Services;

/// <summary>
/// Service interface for URL generation and manipulation
/// </summary>
public interface IUrlService
{
    /// <summary>
    /// Converts a relative path to a full URL using the current request context
    /// </summary>
    /// <param name="relativePath">The relative path to convert</param>
    /// <returns>The full URL</returns>
    string GetFullUrl(string relativePath);

    /// <summary>
    /// Converts a collection of relative paths to full URLs
    /// </summary>
    /// <param name="relativePaths">The relative paths to convert</param>
    /// <returns>The full URLs</returns>
    List<string> GetFullUrls(IEnumerable<string> relativePaths);
}
