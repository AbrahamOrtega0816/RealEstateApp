using RealEstateAPI.Features.Owners.DTOs;
using RealEstateAPI.Features.Shared.DTOs;

namespace RealEstateAPI.Features.Owners.Services;

/// <summary>
/// Interface for owner service operations
/// </summary>
public interface IOwnerService
{
    /// <summary>
    /// Gets all owners with pagination
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paginated list of owners</returns>
    Task<PagedResultDto<OwnerDto>> GetOwnersAsync(int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets an owner by ID
    /// </summary>
    /// <param name="id">Owner ID</param>
    /// <returns>Owner if found, null otherwise</returns>
    Task<OwnerDto?> GetOwnerByIdAsync(string id);

    /// <summary>
    /// Creates a new owner
    /// </summary>
    /// <param name="createOwnerDto">Owner creation data</param>
    /// <returns>Created owner</returns>
    Task<OwnerDto> CreateOwnerAsync(CreateOwnerDto createOwnerDto);

    /// <summary>
    /// Updates an existing owner
    /// </summary>
    /// <param name="id">Owner ID</param>
    /// <param name="updateOwnerDto">Owner update data</param>
    /// <returns>Updated owner if found, null otherwise</returns>
    Task<OwnerDto?> UpdateOwnerAsync(string id, CreateOwnerDto updateOwnerDto);

    /// <summary>
    /// Deletes an owner (soft delete)
    /// </summary>
    /// <param name="id">Owner ID</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteOwnerAsync(string id);
}
