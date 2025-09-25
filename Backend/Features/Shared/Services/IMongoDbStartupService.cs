namespace RealEstateAPI.Features.Shared.Services;

/// <summary>
/// Service for MongoDB startup verification and health checks
/// </summary>
public interface IMongoDbStartupService
{
    /// <summary>
    /// Verifies MongoDB connection during application startup
    /// </summary>
    /// <returns>Task representing the verification operation</returns>
    Task VerifyConnectionAsync();

    /// <summary>
    /// Gets MongoDB connection information
    /// </summary>
    /// <returns>Connection information object</returns>
    Task<object> GetConnectionInfoAsync();

    /// <summary>
    /// Checks if MongoDB is accessible
    /// </summary>
    /// <returns>True if accessible, false otherwise</returns>
    Task<bool> IsAccessibleAsync();
}
