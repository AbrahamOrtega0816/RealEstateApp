using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Features.Properties.DTOs;
using RealEstateAPI.Features.Properties.Services;
using RealEstateAPI.Features.Shared.DTOs;

namespace RealEstateAPI.Features.Properties.Controllers;

/// <summary>
/// Controller for managing properties
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(IPropertyService propertyService, ILogger<PropertiesController> logger)
    {
        _propertyService = propertyService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all properties with pagination and filtering
    /// </summary>
    /// <param name="filter">Filter criteria for properties</param>
    /// <returns>Paginated list of properties</returns>
    /// <response code="200">Returns the paginated list of properties</response>
    /// <response code="400">If the request parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<PropertyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResultDto<PropertyDto>>> GetProperties([FromQuery] PropertyFilterDto filter)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _propertyService.GetPropertiesAsync(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving properties");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while retrieving properties");
        }
    }

    /// <summary>
    /// Gets a specific property by ID
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <returns>Property details</returns>
    /// <response code="200">Returns the property</response>
    /// <response code="404">If the property is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PropertyDto>> GetProperty(string id)
    {
        try
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            
            if (property == null)
            {
                return NotFound($"Property with ID {id} not found");
            }

            return Ok(property);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving property with ID: {PropertyId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while retrieving the property");
        }
    }

    /// <summary>
    /// Creates a new property
    /// </summary>
    /// <param name="createPropertyDto">Property creation data</param>
    /// <returns>Created property</returns>
    /// <response code="201">Returns the newly created property</response>
    /// <response code="400">If the property data is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PropertyDto>> CreateProperty([FromBody] CreatePropertyDto createPropertyDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var property = await _propertyService.CreatePropertyAsync(createPropertyDto);
            
            return CreatedAtAction(
                nameof(GetProperty), 
                new { id = property.Id }, 
                property);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating property");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while creating the property");
        }
    }

    /// <summary>
    /// Updates an existing property
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <param name="updatePropertyDto">Property update data</param>
    /// <returns>Updated property</returns>
    /// <response code="200">Returns the updated property</response>
    /// <response code="400">If the property data is invalid</response>
    /// <response code="404">If the property is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PropertyDto>> UpdateProperty(string id, [FromBody] CreatePropertyDto updatePropertyDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var property = await _propertyService.UpdatePropertyAsync(id, updatePropertyDto);
            
            if (property == null)
            {
                return NotFound($"Property with ID {id} not found");
            }

            return Ok(property);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property with ID: {PropertyId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while updating the property");
        }
    }

    /// <summary>
    /// Deletes a property (soft delete)
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <returns>Success status</returns>
    /// <response code="204">Property deleted successfully</response>
    /// <response code="404">If the property is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProperty(string id)
    {
        try
        {
            var deleted = await _propertyService.DeletePropertyAsync(id);
            
            if (!deleted)
            {
                return NotFound($"Property with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property with ID: {PropertyId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while deleting the property");
        }
    }

    /// <summary>
    /// Change property price with trace
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <param name="newPrice">New property price</param>
    /// <returns>Updated property</returns>
    /// <response code="200">Returns the property with updated price</response>
    /// <response code="400">If the price is invalid</response>
    /// <response code="404">If the property is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("{id}/price")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PropertyDto>> ChangePriceProperty(string id, [FromBody] decimal newPrice)
    {
        try
        {
            if (newPrice <= 0)
            {
                return BadRequest("Price must be greater than 0");
            }

            var property = await _propertyService.ChangePricePropertyAsync(id, newPrice);
            
            if (property == null)
            {
                return NotFound($"Property with ID {id} not found");
            }

            return Ok(property);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing price for property with ID: {PropertyId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while changing the property price");
        }
    }
}
