using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Features.PropertyTraces.DTOs;
using RealEstateAPI.Features.PropertyTraces.Services;

namespace RealEstateAPI.Features.PropertyTraces.Controllers;

/// <summary>
/// Controller for managing property traces (transaction history)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PropertyTracesController : ControllerBase
{
    private readonly IPropertyTraceService _propertyTraceService;
    private readonly ILogger<PropertyTracesController> _logger;

    public PropertyTracesController(IPropertyTraceService propertyTraceService, ILogger<PropertyTracesController> logger)
    {
        _propertyTraceService = propertyTraceService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all traces for a specific property
    /// </summary>
    /// <param name="propertyId">Property ID</param>
    /// <returns>List of property traces</returns>
    /// <response code="200">Returns the list of property traces</response>
    /// <response code="404">If the property is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("property/{propertyId}")]
    [ProducesResponseType(typeof(IEnumerable<PropertyTraceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PropertyTraceDto>>> GetPropertyTraces(string propertyId)
    {
        try
        {
            var traces = await _propertyTraceService.GetTracesByPropertyIdAsync(propertyId);
            return Ok(traces);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving traces for property: {PropertyId}", propertyId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while retrieving property traces");
        }
    }

    /// <summary>
    /// Gets a specific property trace by ID
    /// </summary>
    /// <param name="id">Property trace ID</param>
    /// <returns>Property trace details</returns>
    /// <response code="200">Returns the property trace</response>
    /// <response code="404">If the property trace is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PropertyTraceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PropertyTraceDto>> GetPropertyTrace(string id)
    {
        try
        {
            var trace = await _propertyTraceService.GetTraceByIdAsync(id);
            
            if (trace == null)
            {
                return NotFound($"Property trace with ID {id} not found");
            }

            return Ok(trace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving property trace with ID: {TraceId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while retrieving the property trace");
        }
    }

    /// <summary>
    /// Creates a new property trace
    /// </summary>
    /// <param name="createPropertyTraceDto">Property trace creation data</param>
    /// <returns>Created property trace</returns>
    /// <response code="201">Returns the newly created property trace</response>
    /// <response code="400">If the property trace data is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(PropertyTraceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PropertyTraceDto>> CreatePropertyTrace([FromBody] CreatePropertyTraceDto createPropertyTraceDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var trace = await _propertyTraceService.CreateTraceAsync(createPropertyTraceDto);
            
            return CreatedAtAction(
                nameof(GetPropertyTrace), 
                new { id = trace.Id }, 
                trace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating property trace");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while creating the property trace");
        }
    }

    /// <summary>
    /// Updates an existing property trace
    /// </summary>
    /// <param name="id">Property trace ID</param>
    /// <param name="updatePropertyTraceDto">Property trace update data</param>
    /// <returns>Updated property trace</returns>
    /// <response code="200">Returns the updated property trace</response>
    /// <response code="400">If the property trace data is invalid</response>
    /// <response code="404">If the property trace is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PropertyTraceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PropertyTraceDto>> UpdatePropertyTrace(string id, [FromBody] CreatePropertyTraceDto updatePropertyTraceDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var trace = await _propertyTraceService.UpdateTraceAsync(id, updatePropertyTraceDto);
            
            if (trace == null)
            {
                return NotFound($"Property trace with ID {id} not found");
            }

            return Ok(trace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property trace with ID: {TraceId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while updating the property trace");
        }
    }

    /// <summary>
    /// Deletes a property trace
    /// </summary>
    /// <param name="id">Property trace ID</param>
    /// <returns>Success status</returns>
    /// <response code="204">Property trace deleted successfully</response>
    /// <response code="404">If the property trace is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePropertyTrace(string id)
    {
        try
        {
            var deleted = await _propertyTraceService.DeleteTraceAsync(id);
            
            if (!deleted)
            {
                return NotFound($"Property trace with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property trace with ID: {TraceId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while deleting the property trace");
        }
    }

    /// <summary>
    /// Gets property traces within a date range
    /// </summary>
    /// <param name="propertyId">Property ID</param>
    /// <param name="startDate">Start date for the range</param>
    /// <param name="endDate">End date for the range</param>
    /// <returns>List of property traces within the date range</returns>
    /// <response code="200">Returns the list of property traces</response>
    /// <response code="400">If the date range is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("property/{propertyId}/daterange")]
    [ProducesResponseType(typeof(IEnumerable<PropertyTraceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PropertyTraceDto>>> GetPropertyTracesByDateRange(
        string propertyId, 
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date cannot be greater than end date");
            }

            var traces = await _propertyTraceService.GetTracesByDateRangeAsync(propertyId, startDate, endDate);
            return Ok(traces);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving traces for property {PropertyId} between {StartDate} and {EndDate}", 
                propertyId, startDate, endDate);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while retrieving property traces");
        }
    }
}
