using System.ComponentModel.DataAnnotations;

namespace RealEstateAPI.Features.Properties.DTOs;

/// <summary>
/// DTO for creating new properties
/// </summary>
public class CreatePropertyDto
{
    /// <summary>
    /// Owner identifier
    /// </summary>
    [Required(ErrorMessage = "Owner ID is required")]
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
    [StringLength(50, ErrorMessage = "Code internal cannot exceed 50 characters")]
    public string CodeInternal { get; set; } = string.Empty;

    /// <summary>
    /// Property year of construction
    /// </summary>
    [Range(1800, 2100, ErrorMessage = "Year must be between 1800 and 2100")]
    public int Year { get; set; }
}
