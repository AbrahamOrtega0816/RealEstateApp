using RealEstateAPI.Features.Authentication.Models;
using RealEstateAPI.Features.Authentication.DTOs;
using System.Security.Claims;

namespace RealEstateAPI.Features.Authentication.Services;

/// <summary>
/// Interface for JWT token service
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT access token for the user
    /// </summary>
    /// <param name="user">User information</param>
    /// <returns>JWT token string</returns>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    /// <returns>Refresh token string</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a JWT token and returns claims principal
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>Claims principal if valid, null otherwise</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Gets user ID from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID if valid token, null otherwise</returns>
    string? GetUserIdFromToken(string token);

    /// <summary>
    /// Gets token expiration date
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Expiration date if valid token, null otherwise</returns>
    DateTime? GetTokenExpiration(string token);

    /// <summary>
    /// Checks if token is expired
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>True if expired, false otherwise</returns>
    bool IsTokenExpired(string token);

    /// <summary>
    /// Authenticates user login credentials
    /// </summary>
    /// <param name="loginDto">User login credentials</param>
    /// <returns>Authentication response with tokens</returns>
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registerDto">User registration data</param>
    /// <returns>Authentication response with tokens</returns>
    Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);

    /// <summary>
    /// Refreshes access token using refresh token
    /// </summary>
    /// <param name="refreshTokenDto">Refresh token data</param>
    /// <returns>New authentication response with tokens</returns>
    Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);

    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    /// <param name="token">Refresh token to revoke</param>
    /// <returns>True if revoked successfully</returns>
    Task<bool> RevokeTokenAsync(string token);

    /// <summary>
    /// Gets current authenticated user information
    /// </summary>
    /// <param name="userId">User ID from token</param>
    /// <returns>Current user data</returns>
    Task<UserDto?> GetCurrentUserAsync(string userId);

    /// <summary>
    /// Validates token asynchronously
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>True if token is valid</returns>
    Task<bool> ValidateTokenAsync(string token);
}
