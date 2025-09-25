using RealEstateAPI.Features.PropertyTraces.DTOs;

namespace RealEstateAPI.Features.PropertyTraces.Services;

/// <summary>
/// Interface for property trace service operations
/// </summary>
public interface IPropertyTraceService
{
    /// <summary>
    /// Gets all traces for a specific property
    /// </summary>
    /// <param name="propertyId">Property ID</param>
    /// <returns>List of property traces</returns>
    Task<IEnumerable<PropertyTraceDto>> GetTracesByPropertyIdAsync(string propertyId);

    /// <summary>
    /// Gets a property trace by ID
    /// </summary>
    /// <param name="id">Trace ID</param>
    /// <returns>Property trace if found, null otherwise</returns>
    Task<PropertyTraceDto?> GetTraceByIdAsync(string id);

    /// <summary>
    /// Creates a new property trace
    /// </summary>
    /// <param name="createTraceDto">Trace creation data</param>
    /// <returns>Created property trace</returns>
    Task<PropertyTraceDto> CreateTraceAsync(CreatePropertyTraceDto createTraceDto);

    /// <summary>
    /// Updates an existing property trace
    /// </summary>
    /// <param name="id">Trace ID</param>
    /// <param name="updateTraceDto">Trace update data</param>
    /// <returns>Updated property trace if found, null otherwise</returns>
    Task<PropertyTraceDto?> UpdateTraceAsync(string id, CreatePropertyTraceDto updateTraceDto);

    /// <summary>
    /// Deletes a property trace
    /// </summary>
    /// <param name="id">Trace ID</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteTraceAsync(string id);

    /// <summary>
    /// Gets property traces within a date range for a specific property
    /// </summary>
    /// <param name="propertyId">Property ID</param>
    /// <param name="startDate">Start date for filtering</param>
    /// <param name="endDate">End date for filtering</param>
    /// <returns>List of property traces within the date range</returns>
    Task<IEnumerable<PropertyTraceDto>> GetTracesByDateRangeAsync(string propertyId, DateTime startDate, DateTime endDate);
}
