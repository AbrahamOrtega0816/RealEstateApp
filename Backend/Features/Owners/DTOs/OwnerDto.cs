using System.ComponentModel.DataAnnotations;

namespace RealEstateAPI.Features.Owners.DTOs;

/// <summary>
/// DTO for transferring owner information
/// </summary>
public class OwnerDto
{
    /// <summary>
    /// Unique owner identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Owner name
    /// </summary>
    [Required(ErrorMessage = "Owner name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Owner address
    /// </summary>
    [Required(ErrorMessage = "Address is required")]
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Owner photo URL
    /// </summary>
    [Url(ErrorMessage = "Photo must be a valid URL")]
    public string Photo { get; set; } = string.Empty;

    /// <summary>
    /// Owner birthday
    /// </summary>
    [Required(ErrorMessage = "Birthday is required")]
    public DateTime Birthday { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Owner status
    /// </summary>
    public bool IsActive { get; set; } = true;
}
