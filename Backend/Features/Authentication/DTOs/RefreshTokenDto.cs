using System.ComponentModel.DataAnnotations;

namespace RealEstateAPI.Features.Authentication.DTOs;

/// <summary>
/// DTO for refresh token request
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// Refresh token
    /// </summary>
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}
