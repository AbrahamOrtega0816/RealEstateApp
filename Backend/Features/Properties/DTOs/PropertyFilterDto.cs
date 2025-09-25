using System.ComponentModel.DataAnnotations;

namespace RealEstateAPI.Features.Properties.DTOs;

/// <summary>
/// DTO for filtering properties
/// </summary>
public class PropertyFilterDto
{
    /// <summary>
    /// Filter by property name (partial search)
    /// </summary>
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string? Name { get; set; }

    /// <summary>
    /// Filter by address (partial search)
    /// </summary>
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string? Address { get; set; }

    /// <summary>
    /// Minimum price for filter
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Minimum price must be greater than or equal to 0")]
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Maximum price for filter
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Maximum price must be greater than or equal to 0")]
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Page number for pagination (default 1)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size for pagination (default 10, maximum 100)
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Field to sort by (name, address, price, createdAt)
    /// </summary>
    public string SortBy { get; set; } = "createdAt";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; set; } = "desc";
}
