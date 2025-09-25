using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using RealEstateAPI.Configuration;
using RealEstateAPI.Features.Authentication.Models;
using RealEstateAPI.Features.Authentication.DTOs;

namespace RealEstateAPI.Features.Authentication.Services;

/// <summary>
/// JWT service implementation for authentication and token management
/// </summary>
public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUserService _userService;
    private readonly ILogger<JwtService> _logger;

    public JwtService(
        IOptions<JwtSettings> jwtSettings,
        IUserService userService,
        ILogger<JwtService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _userService = userService;
        _logger = logger;
    }


    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("userId", user.Id ?? string.Empty),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return null;
        }
    }

    public string? GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public DateTime? GetTokenExpiration(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.ValidTo;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error getting token expiration");
            return null;
        }
    }

    public bool IsTokenExpired(string token)
    {
        var expiration = GetTokenExpiration(token);
        return expiration.HasValue && expiration.Value <= DateTime.UtcNow;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", loginDto.Email);
                return null;
            }

            // Check if account is locked
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                _logger.LogWarning("Login attempt for locked account: {Email}", loginDto.Email);
                return null;
            }

            // Reset failed attempts and update last login
            await _userService.ResetFailedLoginAttemptsAsync(user.Id!);
            await _userService.UpdateLastLoginAsync(user.Id!);

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            // Save refresh token
            await UpdateRefreshTokenAsync(user.Id!, refreshToken, loginDto.RememberMe);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = MapToUserDto(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Email}", loginDto.Email);
            throw;
        }
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userService.GetUserByEmailAsync(registerDto.Email);

            if (existingUser != null)
            {
                _logger.LogWarning("Registration attempt with existing email: {Email}", registerDto.Email);
                return null;
            }

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = registerDto.Role,
                IsActive = true,
                IsEmailVerified = false
            };

            user = await _userService.CreateUserAsync(user);

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            // Save refresh token
            await UpdateRefreshTokenAsync(user.Id!, refreshToken, false);

            _logger.LogInformation("User registered successfully: {Email}", registerDto.Email);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = MapToUserDto(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration: {Email}", registerDto.Email);
            throw;
        }
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        try
        {
            var user = await _userService.GetUserByRefreshTokenAsync(refreshTokenDto.RefreshToken);

            if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow || !user.IsActive)
            {
                _logger.LogWarning("Invalid refresh token used");
                return null;
            }

            var accessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            // Update refresh token
            await UpdateRefreshTokenAsync(user.Id!, newRefreshToken, false);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt,
                User = MapToUserDto(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            throw;
        }
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        try
        {
            var user = await _userService.GetUserByRefreshTokenAsync(token);
            if (user == null) return false;

            return await _userService.UpdateRefreshTokenAsync(user.Id!, string.Empty, DateTime.UtcNow.AddDays(-1));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token");
            throw;
        }
    }

    public async Task<UserDto?> GetCurrentUserAsync(string userId)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return user != null ? MapToUserDto(user) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var principal = ValidateToken(token);
            if (principal == null)
                return false;

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userService.GetUserByIdAsync(userId);
            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return false;
        }
    }

    /// <summary>
    /// Updates user's refresh token
    /// </summary>
    private async Task UpdateRefreshTokenAsync(string userId, string refreshToken, bool rememberMe)
    {
        var expiry = rememberMe 
            ? DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays) 
            : DateTime.UtcNow.AddHours(24);

        await _userService.UpdateRefreshTokenAsync(userId, refreshToken, expiry);
    }

    /// <summary>
    /// Maps User entity to UserDto
    /// </summary>
    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id ?? string.Empty,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            Permissions = user.Permissions,
            IsActive = user.IsActive,
            IsEmailVerified = user.IsEmailVerified,
            LastLogin = user.LastLogin,
            CreatedAt = user.CreatedAt
        };
    }
}
