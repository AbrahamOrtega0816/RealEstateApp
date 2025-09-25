using System.ComponentModel.DataAnnotations;

namespace RealEstateAPI.Features.Properties.DTOs;

/// <summary>
/// DTO for transferring property information
/// </summary>
public class PropertyDto
{
    /// <summary>
    /// Unique property identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Owner identifier
    /// </summary>
    public string IdOwner { get; set; } = string.Empty;

    /// <summary>
    /// Property name
    /// </summary>
    [Required(ErrorMessage = "Property name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Property address
    /// </summary>
    [Required(ErrorMessage = "Address is required")]
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Property price
    /// </summary>
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    /// <summary>
    /// Property image URL
    /// </summary>
    [Url(ErrorMessage = "Image must be a valid URL")]
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Internal property code
    /// </summary>
    public string CodeInternal { get; set; } = string.Empty;

    /// <summary>
    /// Property year of construction
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Property status
    /// </summary>
    public bool IsActive { get; set; } = true;
}
