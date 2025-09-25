using System.ComponentModel.DataAnnotations;

namespace RealEstateAPI.Features.Authentication.DTOs;

/// <summary>
/// DTO for user login
/// </summary>
public class LoginDto
{
    /// <summary>
    /// User email
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User password
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Remember me option for extended token expiry
    /// </summary>
    public bool RememberMe { get; set; } = false;
}
