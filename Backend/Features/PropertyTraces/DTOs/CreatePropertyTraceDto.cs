using System.ComponentModel.DataAnnotations;

namespace RealEstateAPI.Features.PropertyTraces.DTOs;

/// <summary>
/// DTO for creating new property traces
/// </summary>
public class CreatePropertyTraceDto
{
    /// <summary>
    /// Date of the sale/transaction
    /// </summary>
    [Required(ErrorMessage = "Sale date is required")]
    public DateTime DateSale { get; set; }

    /// <summary>
    /// Transaction name or description
    /// </summary>
    [Required(ErrorMessage = "Transaction name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Transaction value
    /// </summary>
    [Required(ErrorMessage = "Value is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
    public decimal Value { get; set; }

    /// <summary>
    /// Tax amount
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Tax must be greater than or equal to 0")]
    public decimal Tax { get; set; }

    /// <summary>
    /// Property identifier
    /// </summary>
    [Required(ErrorMessage = "Property ID is required")]
    public string IdProperty { get; set; } = string.Empty;
}
