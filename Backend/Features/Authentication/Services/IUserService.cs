using RealEstateAPI.Features.Authentication.Models;
using RealEstateAPI.Features.Authentication.DTOs;

namespace RealEstateAPI.Features.Authentication.Services;

/// <summary>
/// Interface for user management operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user in the database
    /// </summary>
    /// <param name="user">User to create</param>
    /// <returns>Created user</returns>
    Task<User> CreateUserAsync(User user);

    /// <summary>
    /// Gets a user by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetUserByIdAsync(string id);

    /// <summary>
    /// Updates a user's information
    /// </summary>
    /// <param name="user">User to update</param>
    /// <returns>Updated user</returns>
    Task<User> UpdateUserAsync(User user);

    /// <summary>
    /// Updates user's refresh token
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="refreshToken">New refresh token</param>
    /// <param name="refreshTokenExpiry">Token expiry date</param>
    /// <returns>Success status</returns>
    Task<bool> UpdateRefreshTokenAsync(string userId, string refreshToken, DateTime refreshTokenExpiry);

    /// <summary>
    /// Gets user by refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Updates user's last login time
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success status</returns>
    Task<bool> UpdateLastLoginAsync(string userId);

    /// <summary>
    /// Increments failed login attempts
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success status</returns>
    Task<bool> IncrementFailedLoginAttemptsAsync(string userId);

    /// <summary>
    /// Resets failed login attempts
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success status</returns>
    Task<bool> ResetFailedLoginAttemptsAsync(string userId);

    /// <summary>
    /// Locks user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="lockoutEnd">Lockout end time</param>
    /// <returns>Success status</returns>
    Task<bool> LockUserAccountAsync(string userId, DateTime lockoutEnd);
}
