namespace RealEstateAPI.Features.Authentication.DTOs;

/// <summary>
/// DTO for authentication response
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token for renewing access token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// User information
    /// </summary>
    public UserDto User { get; set; } = new();
}
