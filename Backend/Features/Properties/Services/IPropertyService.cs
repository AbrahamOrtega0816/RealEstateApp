using RealEstateAPI.Features.Properties.DTOs;
using RealEstateAPI.Features.Shared.DTOs;

namespace RealEstateAPI.Features.Properties.Services;

/// <summary>
/// Interface for the property service
/// </summary>
public interface IPropertyService
{
    /// <summary>
    /// Gets all properties with filters and pagination
    /// </summary>
    /// <param name="filter">Search filters</param>
    /// <returns>Paginated result of properties</returns>
    Task<PagedResultDto<PropertyDto>> GetPropertiesAsync(PropertyFilterDto filter);

    /// <summary>
    /// Gets a property by its ID
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <returns>Found property or null</returns>
    Task<PropertyDto?> GetPropertyByIdAsync(string id);

    /// <summary>
    /// Creates a new property
    /// </summary>
    /// <param name="createPropertyDto">New property data</param>
    /// <returns>Created property</returns>
    Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto);

    /// <summary>
    /// Updates an existing property
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <param name="updatePropertyDto">Updated data</param>
    /// <returns>Updated property or null if not exists</returns>
    Task<PropertyDto?> UpdatePropertyAsync(string id, CreatePropertyDto updatePropertyDto);

    /// <summary>
    /// Deletes a property (soft delete)
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeletePropertyAsync(string id);

    /// <summary>
    /// Changes the price of an existing property
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <param name="newPrice">New price for the property</param>
    /// <returns>Updated property with new price or null if not exists</returns>
    Task<PropertyDto?> ChangePricePropertyAsync(string id, decimal newPrice);
}
