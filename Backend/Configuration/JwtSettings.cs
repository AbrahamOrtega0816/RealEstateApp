namespace RealEstateAPI.Configuration;

/// <summary>
/// Configuration for JWT tokens
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Secret key for signing JWT tokens
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Token issuer
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Token audience
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Access token expiration time in minutes
    /// </summary>
    public int ExpirationInMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh token expiration time in days
    /// </summary>
    public int RefreshTokenExpirationInDays { get; set; } = 7;
}
