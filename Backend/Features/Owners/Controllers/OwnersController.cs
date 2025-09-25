using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Features.Owners.DTOs;
using RealEstateAPI.Features.Owners.Services;
using RealEstateAPI.Features.Shared.DTOs;

namespace RealEstateAPI.Features.Owners.Controllers;

/// <summary>
/// Controller for managing owners
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OwnersController : ControllerBase
{
    private readonly IOwnerService _ownerService;
    private readonly ILogger<OwnersController> _logger;

    public OwnersController(IOwnerService ownerService, ILogger<OwnersController> logger)
    {
        _ownerService = ownerService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all owners with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <returns>Paginated list of owners</returns>
    /// <response code="200">Returns the paginated list of owners</response>
    /// <response code="400">If the request parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<OwnerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResultDto<OwnerDto>>> GetOwners(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageNumber < 1)
            {
                return BadRequest("Page number must be greater than 0");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Page size must be between 1 and 100");
            }

            var result = await _ownerService.GetOwnersAsync(pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving owners");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while retrieving owners");
        }
    }

    /// <summary>
    /// Gets a specific owner by ID
    /// </summary>
    /// <param name="id">Owner ID</param>
    /// <returns>Owner details</returns>
    /// <response code="200">Returns the owner</response>
    /// <response code="404">If the owner is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OwnerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OwnerDto>> GetOwner(string id)
    {
        try
        {
            var owner = await _ownerService.GetOwnerByIdAsync(id);
            
            if (owner == null)
            {
                return NotFound($"Owner with ID {id} not found");
            }

            return Ok(owner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving owner with ID: {OwnerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while retrieving the owner");
        }
    }

    /// <summary>
    /// Creates a new owner
    /// </summary>
    /// <param name="createOwnerDto">Owner creation data</param>
    /// <returns>Created owner</returns>
    /// <response code="201">Returns the newly created owner</response>
    /// <response code="400">If the owner data is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(OwnerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OwnerDto>> CreateOwner([FromBody] CreateOwnerDto createOwnerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var owner = await _ownerService.CreateOwnerAsync(createOwnerDto);
            
            return CreatedAtAction(
                nameof(GetOwner), 
                new { id = owner.Id }, 
                owner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating owner");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while creating the owner");
        }
    }

    /// <summary>
    /// Updates an existing owner
    /// </summary>
    /// <param name="id">Owner ID</param>
    /// <param name="updateOwnerDto">Owner update data</param>
    /// <returns>Updated owner</returns>
    /// <response code="200">Returns the updated owner</response>
    /// <response code="400">If the owner data is invalid</response>
    /// <response code="404">If the owner is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(OwnerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OwnerDto>> UpdateOwner(string id, [FromBody] CreateOwnerDto updateOwnerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var owner = await _ownerService.UpdateOwnerAsync(id, updateOwnerDto);
            
            if (owner == null)
            {
                return NotFound($"Owner with ID {id} not found");
            }

            return Ok(owner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating owner with ID: {OwnerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while updating the owner");
        }
    }

    /// <summary>
    /// Deletes an owner (soft delete)
    /// </summary>
    /// <param name="id">Owner ID</param>
    /// <returns>Success status</returns>
    /// <response code="204">Owner deleted successfully</response>
    /// <response code="404">If the owner is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteOwner(string id)
    {
        try
        {
            var deleted = await _ownerService.DeleteOwnerAsync(id);
            
            if (!deleted)
            {
                return NotFound($"Owner with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting owner with ID: {OwnerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while deleting the owner");
        }
    }
}
