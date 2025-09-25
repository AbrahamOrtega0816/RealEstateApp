using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RealEstateAPI.Features.Authentication.DTOs;
using RealEstateAPI.Features.Authentication.Services;

namespace RealEstateAPI.Features.Authentication.Controllers;

/// <summary>
/// Controller for user authentication and authorization
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IJwtService jwtService, ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens
    /// </summary>
    /// <param name="loginDto">User login credentials</param>
    /// <returns>Authentication response with tokens</returns>
    /// <response code="200">Returns the authentication tokens</response>
    /// <response code="400">If the login data is invalid</response>
    /// <response code="401">If the credentials are incorrect</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authResponse = await _jwtService.LoginAsync(loginDto);
            
            if (authResponse == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Email}", loginDto.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred during login");
        }
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registerDto">User registration data</param>
    /// <returns>Authentication response with tokens</returns>
    /// <response code="201">Returns the authentication tokens for the new user</response>
    /// <response code="400">If the registration data is invalid</response>
    /// <response code="409">If the user already exists</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authResponse = await _jwtService.RegisterAsync(registerDto);
            
            if (authResponse == null)
            {
                return Conflict("User with this email already exists");
            }

            return CreatedAtAction(nameof(GetProfile), null, authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user: {Email}", registerDto.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred during registration");
        }
    }

    /// <summary>
    /// Refreshes the access token using a refresh token
    /// </summary>
    /// <param name="refreshTokenDto">Refresh token data</param>
    /// <returns>New authentication response with tokens</returns>
    /// <response code="200">Returns the new authentication tokens</response>
    /// <response code="400">If the refresh token data is invalid</response>
    /// <response code="401">If the refresh token is invalid or expired</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authResponse = await _jwtService.RefreshTokenAsync(refreshTokenDto);
            
            if (authResponse == null)
            {
                return Unauthorized("Invalid or expired refresh token");
            }

            return Ok(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred during token refresh");
        }
    }

    /// <summary>
    /// Revokes a refresh token (logout)
    /// </summary>
    /// <param name="refreshTokenDto">Refresh token to revoke</param>
    /// <returns>Success status</returns>
    /// <response code="200">Token revoked successfully</response>
    /// <response code="400">If the refresh token data is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _jwtService.RevokeTokenAsync(refreshTokenDto.RefreshToken);
            
            if (!result)
            {
                return BadRequest("Failed to revoke token");
            }

            return Ok(new { message = "Token revoked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token revocation");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred during token revocation");
        }
    }

    /// <summary>
    /// Gets the current user's profile information
    /// </summary>
    /// <returns>User profile information</returns>
    /// <response code="200">Returns the user profile</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        try
        {
            // Get user ID from authenticated context
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var userProfile = await _jwtService.GetCurrentUserAsync(userId);
            
            if (userProfile == null)
            {
                return Unauthorized("User not found");
            }

            return Ok(userProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred while retrieving user profile");
        }
    }

    /// <summary>
    /// Validates if a JWT token is valid and not expired
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>Token validation result</returns>
    /// <response code="200">Token is valid</response>
    /// <response code="400">If the token is invalid or expired</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ValidateToken([FromBody] string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required");
            }

            var isValid = await _jwtService.ValidateTokenAsync(token);
            
            if (!isValid)
            {
                return BadRequest("Token is invalid or expired");
            }

            return Ok(new { message = "Token is valid", isValid = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An error occurred during token validation");
        }
    }
}
