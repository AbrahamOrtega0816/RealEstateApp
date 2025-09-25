namespace RealEstateAPI.Features.Authentication.DTOs;

/// <summary>
/// DTO for user information (without sensitive data)
/// </summary>
public class UserDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// User role
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// User permissions
    /// </summary>
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// Account status
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Email verification status
    /// </summary>
    public bool IsEmailVerified { get; set; }

    /// <summary>
    /// Last login date
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Account creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
