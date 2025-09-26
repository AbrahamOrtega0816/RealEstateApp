using System.ComponentModel.DataAnnotations;

namespace RealEstateAPI.Features.Owners.DTOs;

/// <summary>
/// DTO for creating new owners
/// </summary>
public class CreateOwnerDto
{
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
    /// Owner birthday
    /// </summary>
    [Required(ErrorMessage = "Birthday is required")]
    public DateTime Birthday { get; set; }
}
